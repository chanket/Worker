using Common.Frames;
using Common.Server.Helpers;
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

namespace Server.Forms
{
    public partial class RunForm : Form
    {
        private IEnumerable<ClientData> Clients { get; }

        private RunForm()
        {
            InitializeComponent();
        }

        public RunForm(IEnumerable<ClientData> clients)
            :this()
        {
            Clients = clients;
        }

        private void FormShell_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (textBoxFileName.Text.Length > 0)
            {
                button1.Enabled = false;

                foreach (var client in Clients)
                {
                    try
                    {
                        using (var helper = new CommandHelper(client))
                        {
                            await helper.StartAsync(
                                textBoxFileName.Text,
                                textBoxArguments.Text,
                                checkBoxHide.Checked,
                                false, false, false,
                                checkBoxStart.Checked ? dateTimePicker1.Value.ToUniversalTime() : new DateTime(0),
                                checkBoxStop.Checked ? dateTimePicker2.Value.ToUniversalTime() : new DateTime(0)
                                );
                        }
                    }
                    catch
                    {

                    }
                }

                Close();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Enabled = checkBoxStart.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Enabled = checkBoxStop.Checked;
        }
    }
}
