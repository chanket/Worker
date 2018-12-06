using Common.Frames;
using Server.Forms.Dialogs;
using Common.Server.Helpers;
using Server.Settings;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Server.Forms
{
    public partial class PanelForm : Form
    {
        #region Handler
        protected void OnConnect(ClientData client)
        {
            var ep = client.TcpClient.Client.RemoteEndPoint;
            var info = client.Information;
            this.Invoke(new MethodInvoker(() => {
                ListViewItem lvi = new ListViewItem(ep.ToString());
                lvi.Tag = client;
                lvi.SubItems.Add(info.ComputerName);
                lvi.SubItems.Add(info.Version);
                lvi.SubItems.Add(info.Os);
                lvi.SubItems.Add(info.Cpus[0]);
                lvi.SubItems.Add(info.Cpus.Count.ToString());
                lvi.SubItems.Add((info.Memory / 1048576).ToString() + "MB");
                lvi.SubItems.Add(string.Join(", ", info.Gpus));
                listView1.Items.Add(lvi);
            }));
        }

        protected void OnDisconnect(ClientData client)
        {
            this.Invoke(new MethodInvoker(() => {
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (listView1.Items[i].Tag as ClientData == client)
                    {
                        listView1.Items.RemoveAt(i);
                        break;
                    }
                }
            }));
        }
        #endregion

        public PanelForm()
        {
            InitializeComponent();
        }

        Server Server { get; } = new Server();

        private void Form1_Load(object sender, EventArgs e)
        {
            Server.OnConnect += OnConnect;
            Server.OnDisconnect += OnDisconnect;
            Server.Start();

            //加载自定义命令
            var commands = CommandSetting.Load(@"CommandSettings.xml");
            foreach (var command in commands)
            {
                var item = toolStripMenuItemCMDs.DropDownItems.Add(command.Name);
                item.Tag = command;
                item.Click += toolStripMenuItemCMDs_Click;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            listView1.Width = this.Width - 40;
            listView1.Height = this.Height - 63;
        }

        #region Strip1

        #region 注销关机和重启

        /// <summary>
        /// 注销。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void toolStripMenuItemLock_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要注销选中的机机吗？", "确认", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                foreach (ListViewItem lvi in listView1.SelectedItems)
                {
                    try
                    {
                        ClientData client = lvi.Tag as ClientData;
                        using (SessionCommandHelper helper = new SessionCommandHelper(client))
                        {
                            await helper.StartAsync("rundll32.exe", "user32.dll,LockWorkStation");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 关机。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void toolStripMenuItemShutdown_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要关闭选中的机机吗？", "确认", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                foreach (ListViewItem lvi in listView1.SelectedItems)
                {
                    try
                    {
                        using (SessionCommandHelper helper = new SessionCommandHelper(lvi.Tag as ClientData))
                        {
                            await helper.StartAsync("shutdown", "-s -t 0 -f");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 重启。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void toolStripMenuItemRestart_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要重启选中的机机吗？", "确认", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                foreach (ListViewItem lvi in listView1.SelectedItems)
                {
                    try
                    {
                        using (SessionCommandHelper helper = new SessionCommandHelper(lvi.Tag as ClientData))
                        {
                            await helper.StartAsync("shutdown", "-r -t 0 -f");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        #endregion

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && listView1.SelectedItems.Count >= 1)
            {
                contextMenuStrip1.Show(sender as Control, e.X, e.Y);
            }
        }

        /// <summary>
        /// 截屏。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemScreenshot_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                ScreenshotForm form = new ScreenshotForm(lvi.Tag as ClientData);
                form.Show();
            }
        }

        /// <summary>
        /// 摄像头。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemCamera_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                CameraForm form = new CameraForm(lvi.Tag as ClientData);
                form.Show();
            }
        }


        /// <summary>
        /// 进程列表。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemTaskList_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                TaskListForm form = new TaskListForm(lvi.Tag as ClientData);
                form.Show();
            }
        }

        /// <summary>
        /// 运行。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemRun_Click(object sender, EventArgs e)
        {
            List<ClientData> clients = new List<ClientData>();
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                clients.Add(lvi.Tag as ClientData);
            }

            RunForm form = new RunForm(clients);
            form.Show();
        }

        /// <summary>
        /// 命令行。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemCMD_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                try
                {
                    CmdForm form = new CmdForm(lvi.Tag as ClientData);
                    form.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 快捷命令行。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemCMDs_Click(object sender, EventArgs e)
        {
            CommandSetting setting = (sender as ToolStripMenuItem).Tag as CommandSetting;
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                try
                {
                    CmdForm form = new CmdForm(lvi.Tag as ClientData);
                    form.Show(setting.Commands);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 上传。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemUpload_Click(object sender, EventArgs e)
        {
            List<ClientData> clients = new List<ClientData>();
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                clients.Add(lvi.Tag as ClientData);
            }

            UploadForm form = new UploadForm(clients);
            form.Show();
        }

        /// <summary>
        /// 下载。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemDownload_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                try
                {
                    DownloadForm form = new DownloadForm(lvi.Tag as ClientData);
                    form.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// HTTP下载。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemHttpDownload_Click(object sender, EventArgs e)
        {
            List<ClientData> clients = new List<ClientData>();
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                clients.Add(lvi.Tag as ClientData);
            }

            HttpDownloadForm form = new HttpDownloadForm(clients);
            form.Show();
        }
        #endregion

        #region Keys
        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                RefreshInformation();
            }
        }

        private async void RefreshInformation()
        {
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                try
                {
                    ClientData client = lvi.Tag as ClientData;
                    Common.Frames.Information.RequestFrame frame = new Common.Frames.Information.RequestFrame();
                    await Server.Send(client, frame);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion

    }
}
