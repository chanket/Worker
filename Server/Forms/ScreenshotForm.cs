using Common.Frames;
using Common.Frames.Screenshot;
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
    public partial class ScreenshotForm : Form
    {
        private ClientData Client { get; }

        private ScreenshotForm()
        {
            InitializeComponent();
        }

        public ScreenshotForm(ClientData client)
            : this()
        {
            Client = client;
        }

        private async Task UpdateScreenshot()
        {
            try
            {
                using (ScreenshotHelper helper = new ScreenshotHelper(Client))
                {
                    pictureBox1.Image = new Bitmap(new MemoryStream(await helper.StartAsync()));
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed) MessageBox.Show(ex.Message);
            }
        }

        private async void ScreenshotForm_Load(object sender, EventArgs e)
        {
            await UpdateScreenshot();
        }

        private void ScreenshotForm_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Width = this.Width - 40;
            pictureBox1.Height = this.Height - 63;
        }

        private async void ScreenshotForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F5:
                    {
                        await UpdateScreenshot();
                    }
                    break;
            }
        }
    }
}
