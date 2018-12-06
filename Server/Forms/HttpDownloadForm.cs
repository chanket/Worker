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
    public partial class HttpDownloadForm : Form
    {
        private bool AutoComplete { get; set; } = true;

        protected IEnumerable<ClientData> Clients { get; }

        private HttpDownloadForm()
        {
            InitializeComponent();
        }

        public HttpDownloadForm(IEnumerable<ClientData> clients)
            : this()
        {
            Clients = clients;
        }

        private void HttpDownloadForm_Load(object sender, EventArgs e)
        {
            if (Clients.Count() == 0) button1.Enabled = false;
            InitListView(Clients);
        }

        private void HttpDownloadForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void textBoxLocal_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBoxUrl.Text = string.Join("|", ofd.FileNames);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            textBoxRemote.Enabled = false;
            textBoxUrl.Enabled = false;

            try
            {
                Uri uri = new Uri(textBoxUrl.Text);
                if (uri.Scheme.ToLower() != "http" && uri.Scheme.ToLower() != "https")
                {
                    throw new Exception("不支持的协议类型：" + textBoxUrl.Text);
                }

                List<HttpDownloadHelper> helpers = new List<HttpDownloadHelper>();
                foreach (ClientData client in Clients)
                {
                    if (!IsListViewSuccess(client))
                    {
                        helpers.Add(new HttpDownloadHelper(client));
                    }
                }
                if (helpers.Count != 0)
                {
                    await Download(helpers, textBoxUrl.Text, textBoxRemote.Text, checkBox1.Checked);
                }

                helpers.Clear();
            }
            catch (Exception ex)
            {
                if (this.IsDisposed) MessageBox.Show(ex.Message);
            }

            button1.Enabled = true;
            textBoxRemote.Enabled = true;
            textBoxUrl.Enabled = true;
        }

        #region Progress And Speed Timer
        private void timerSpeed_Tick(object sender, EventArgs e)
        {
            var tag = timerSpeed.Tag as List<HttpDownloadHelper>;

            foreach (var helper in tag)
            {
                if (!IsListViewSuccess(helper.Client as ClientData) && !IsListViewError(helper.Client as ClientData))
                {
                    SetListViewSpeed(helper.Client as ClientData, (helper.Speed / 1024) + " KiB/s");
                }
            }
        }

        private void timerProgress_Tick(object sender, EventArgs e)
        {
            var tag = timerProgress.Tag as List<HttpDownloadHelper>;

            foreach (var helper in tag)
            {
                if (!IsListViewSuccess(helper.Client as ClientData) && !IsListViewError(helper.Client as ClientData))
                {
                    if (helper.Size == -1)
                    {
                        SetListViewProcess(helper.Client as ClientData, (helper.TransferedSize / 1024) + " KiB");
                    }
                    else
                    {
                        SetListViewProcess(helper.Client as ClientData, ((int)(100 * helper.TransferedSize / helper.Size)).ToString() + "%");
                    }
                }
            }
        }
        #endregion

        private async Task Download(List<HttpDownloadHelper> helpers, string url, string remoteFile, bool execute)
        {
            timerSpeed.Tag = helpers;
            timerProgress.Tag = helpers;
            timerSpeed.Enabled = true;
            timerProgress.Enabled = true;

            List<Task> tasks = new List<Task>();
            foreach (var helper in helpers)
            {
                tasks.Add(Download(helper, url, remoteFile, execute));
            }
            await Task.WhenAll(tasks);

            timerSpeed.Enabled = false;
            timerProgress.Enabled = false;
        }

        private async Task Download(HttpDownloadHelper helper, string url, string remoteFile, bool execute)
        {
            try
            {
                SetListViewReady(helper.Client as ClientData);
                SetListViewSpeed(helper.Client as ClientData, "");
                await helper.StartAsync(url, remoteFile);
                SetListViewSuccess(helper.Client as ClientData);

                if (execute)
                {
                    using (CommandHelper helper2 = new CommandHelper(helper.Client))
                    {
                        await helper2.StartAsync(textBoxRemote.Text, null);
                    }
                }
            }
            catch (Exception ex)
            {
                SetListViewError(helper.Client as ClientData, ex.Message);
            }
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
