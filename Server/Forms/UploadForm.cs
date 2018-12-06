using Common;
using Common.Frames;
using Common.Frames.File;
using Common.Server.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.Forms
{
    public partial class UploadForm : Form
    {
        protected IEnumerable<ClientData> Clients { get; }

        protected List<Tuple<FileStream, string>> PendingFiles { get; } = new List<Tuple<FileStream, string>>();

        private UploadForm()
        {
            InitializeComponent();
        }

        public UploadForm(IEnumerable<ClientData> clients)
            : this()
        {
            Clients = clients;
        }

        private void UploadForm_Load(object sender, EventArgs e)
        {
            if (Clients.Count() == 0) button1.Enabled = false;
            InitListView(Clients);
        }

        private void UploadForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var file in PendingFiles)
            {
                file.Item1.Close();
            }
        }

        private void textBoxLocal_TextChanged(object sender, EventArgs e)
        {
            foreach (ClientData tc in Clients)
            {
                SetListViewReady(tc);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBoxLocal.Text = string.Join("|", ofd.FileNames);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            textBoxRemote.Enabled = false;
            textBoxLocal.Enabled = false;
            button2.Enabled = false;

            try
            {
                PendingFiles.ForEach((Tuple<FileStream, string> element) => { element.Item1.Close(); });
                PendingFiles.Clear();
                foreach (var file in textBoxLocal.Text.Split('|'))
                {
                    FileInfo fi = new FileInfo(file);
                    PendingFiles.Add(Tuple.Create(fi.OpenRead(), fi.Name));
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed) MessageBox.Show(ex.Message);
            }

            foreach (ClientData client in Clients)
            {
                if (!IsListViewSuccess(client))
                {
                    using (UploadHelper helper = new UploadHelper(client))
                    {
                        await Upload(helper, textBoxRemote.Text, PendingFiles, checkBox1.Checked);
                    }
                }
            }

            button1.Enabled = true;
            textBoxRemote.Enabled = true;
            textBoxLocal.Enabled = true;
            button2.Enabled = true;
        }

        #region Progress And Speed Timer
        class TimerTag
        {
            public UploadHelper Helper { get; set; } = null;
            public long TotalSize { get; set; } = 0;
            public long BeforeTransferedSize { get; set; } = 0;
            public long TotalTransferedSize
            {
                get
                {
                    return BeforeTransferedSize + (Helper == null ? 0 : Helper.TransferedSize);
                }
            }
        }

        private void timerSpeed_Tick(object sender, EventArgs e)
        {
            var tag = timerSpeed.Tag as TimerTag;

            SetListViewSpeed(tag.Helper.Client as ClientData, (tag.Helper.Speed / 1024).ToString() + "KiB/s");
        }

        private void timerProgress_Tick(object sender, EventArgs e)
        {
            var tag = timerProgress.Tag as TimerTag;

            SetListViewProcess(tag.Helper.Client as ClientData, ((int)(100 * tag.TotalTransferedSize / tag.TotalSize)).ToString() + "%");
        }
        #endregion

        private async Task Upload(UploadHelper helper, string remoteDir, List<Tuple<FileStream, string>> localFiles, bool execute)
        {
            TimerTag timerTag = new TimerTag() { TotalSize = localFiles.Sum((Tuple<FileStream, string> e) => { return e.Item1.Length; }) };
            timerSpeed.Tag = timerTag;
            timerProgress.Tag = timerTag;
            timerSpeed.Enabled = true;
            timerProgress.Enabled = true;

            try
            {
                SetListViewReady(helper.Client as ClientData);
                SetListViewSpeed(helper.Client as ClientData, "");
                foreach (var file in localFiles)
                {
                    timerTag.Helper = helper;
                    {
                        file.Item1.Position = 0;
                        await helper.ToStreamAsync(file.Item1, remoteDir + "\\" + file.Item2);
                        timerTag.BeforeTransferedSize += file.Item1.Length;
                    }
                    timerTag.Helper = null;
                }
                
                SetListViewSuccess(helper.Client as ClientData);
                if (execute)
                {
                    foreach (var file in localFiles)
                    {
                        try
                        {
                            using (CommandHelper helper2 = new CommandHelper(helper.Client))
                            {
                                await helper2.StartAsync(textBoxRemote.Text + "\\" + file.Item2, null);
                            }
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                SetListViewError(helper.Client as ClientData, ex.Message);
            }

            timerSpeed.Enabled = false;
            timerProgress.Enabled = false;
        }

        #region ListView
        private void InitListView(IEnumerable<ClientData> clients)
        {
            foreach (ClientData client in Clients)
            {
                listView1.Items.Add(new ListViewItem(new string[] { client.TcpClient.Client.RemoteEndPoint.ToString(), "0%", "Ready" })
                {
                    Tag = client,
                });
            }
        }

        private ListViewItem FindListViewItem(ClientData client)
        {
            foreach (ListViewItem lvi in listView1.Items)
            {
                if (lvi.Tag == client) return lvi;
            }
            return null;
        }

        private void SetListViewProcess(ClientData client, string process)
        {
            if (IsListViewError(client)) return;
            ListViewItem lvi = FindListViewItem(client);

            if (lvi != null)
            {
                lvi.SubItems[FindListViewItem(client).SubItems.Count - 2] = new ListViewItem.ListViewSubItem(lvi, process);
            }
        }

        private void SetListViewReady(ClientData client)
        {
            ListViewItem lvi = FindListViewItem(client);

            if (lvi != null)
            {
                lvi.SubItems[FindListViewItem(client).SubItems.Count - 1] = new ListViewItem.ListViewSubItem(lvi, "Ready");
                lvi.SubItems[FindListViewItem(client).SubItems.Count - 2] = new ListViewItem.ListViewSubItem(lvi, "0%");
            }
        }

        private void SetListViewSpeed(ClientData client, string speed)
        {
            if (IsListViewError(client)) return;
            ListViewItem lvi = FindListViewItem(client);

            if (lvi != null)
            {
                lvi.SubItems[FindListViewItem(client).SubItems.Count - 1] = new ListViewItem.ListViewSubItem(lvi, speed);
            }
        }

        private void SetListViewError(ClientData client, string message)
        {
            ListViewItem lvi = FindListViewItem(client);

            if (lvi != null)
            {
                lvi.SubItems[FindListViewItem(client).SubItems.Count - 1] = new ListViewItem.ListViewSubItem(lvi, "Error: " + message);
            }
        }

        private void SetListViewSuccess(ClientData client)
        {
            if (IsListViewError(client)) return;
            ListViewItem lvi = FindListViewItem(client);

            if (lvi != null)
            {
                lvi.SubItems[FindListViewItem(client).SubItems.Count - 1] = new ListViewItem.ListViewSubItem(lvi, "Success");
                lvi.SubItems[FindListViewItem(client).SubItems.Count - 2] = new ListViewItem.ListViewSubItem(lvi, "100%");
            }
        }

        private bool IsListViewSuccess(ClientData client)
        {
            ListViewItem lvi = FindListViewItem(client);

            if (lvi != null)
            {
                return lvi.SubItems[FindListViewItem(client).SubItems.Count - 1].Text == "Success";
            }

            return true;
        }

        private bool IsListViewError(ClientData client)
        {
            ListViewItem lvi = FindListViewItem(client);

            if (lvi != null)
            {
                return lvi.SubItems[FindListViewItem(client).SubItems.Count - 1].Text.StartsWith("Error: ");
            }

            return true;
        }
        #endregion

    }
}
