using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Frames;
using Common.Frames.File;
using System.IO;
using System.Threading;
using Server.Forms.Dialogs;
using Common.Server.Helpers;
using Common;

namespace Server.Forms
{
    public partial class DownloadForm : Form
    {
        private DownloadForm()
        {
            InitializeComponent();
        }

        private DownloadHelper Helper { get; set; } = null;

        private FileStream FileStream { get; set; } = null;

        public DownloadForm(ClientData client)
            : this()
        {
            RemoteFileDialog rfd = new RemoteFileDialog(client);
            if (rfd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(rfd.Selected.FullName))
            {
                SaveFileDialog ofd = new SaveFileDialog();
                if (!string.IsNullOrEmpty(rfd.Selected.Extension)) ofd.Filter = rfd.Selected.Extension + " Files|*" + rfd.Selected.Extension;
                ofd.Filter += "|All Files|*.*";
                ofd.FileName = rfd.Selected.Name;

                if (ofd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(ofd.FileName))
                {
                    Helper = new DownloadHelper(client);
                    textBoxRemote.Text = rfd.Selected.FullName;
                    textBoxLocal.Text = ofd.FileName;
                }
            }
        }

        private async void DownloadForm_Load(object sender, EventArgs e)
        {
            if (Helper == null)
            {
                Close();
            }
            else
            {
                timerSpeed.Enabled = true;
                timerProgress.Enabled = true;

                try
                {
                    FileInfo fi = new FileInfo(textBoxLocal.Text);
                    DirectoryInfo di = fi.Directory;

                    if (!di.Exists) di.Create();
                    FileStream = fi.Open(FileMode.OpenOrCreate, FileAccess.Write);

                    await Helper.StartAsync(textBoxRemote.Text, FileStream);
                }
                catch (Exception ex)
                {
                    if (!this.IsDisposed) MessageBox.Show(ex.Message);
                }
            }
        }

        private void DownloadForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FileStream?.Close();
            Helper?.Dispose();
        }

        private void timerSpeed_Tick(object sender, EventArgs e)
        {
            labelSpeed.Text = (Helper.Speed / 1024).ToString() + "KiB/s";
        }

        private void timerProgress_Tick(object sender, EventArgs e)
        {
            if (Helper.Size != -1)
            {
                progressBar1.Value = (int)(progressBar1.Maximum * Helper.TransferedSize / Helper.Size);
                if (Helper.TransferedSize >= Helper.Size) Close();
            }
        }
    }
}
