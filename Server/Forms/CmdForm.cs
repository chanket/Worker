using Common.Frames;
using Common.Frames.Command;
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
    public partial class CmdForm : Form
    {
        private CommandHelper Helper { get; }

        private CmdForm()
        {
            InitializeComponent();
        }

        public CmdForm(ClientData client)
            : this()
        {
            Helper = new CommandHelper(client);
        }

        public async void Show(IEnumerable<string> commands)
        {
            try
            {
                base.Show();
                await Helper.WriteStandardInputAsync(string.Join("\n", commands) + "\n").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void CmdForm_Load(object sender, EventArgs e)
        {
            try
            {
                if ((Helper.Client as ClientData).Information.Os.StartsWith("Microsoft"))
                {
                    await Helper.StartAsync("cmd.exe", null, true, true, true, true);
                    HandleStandardOutput();
                    HandleStandardError();
                }
                else if ((Helper.Client as ClientData).Information.Os.StartsWith("Linux"))
                {
                    await Helper.StartAsync("bash", null, true, true, true, true);
                    await Helper.WriteStandardInputAsync("set -v on\n");
                    HandleStandardOutput();
                    HandleStandardError();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void CmdForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                await Helper.KillAsync();
            }
            catch (Exception ex)
            {
            }
            Helper.Dispose();
        }

        private async void HandleStandardOutput()
        {
            try
            {
                while (true)
                {
                    string data = await Helper.ReadStandardOutputAsync();

                    if (richTextBox1.IsDisposed) break;
                    richTextBox1.AppendText(data);
                    richTextBox1.ScrollToCaret();
                }
            }
            catch { }
        }

        private async void HandleStandardError()
        {
            try
            {
                while (true)
                {
                    string data = await Helper.ReadStandardErrorAsync();

                    if (richTextBox1.IsDisposed) break;
                    richTextBox1.AppendText(data);
                    richTextBox1.ScrollToCaret();
                }
            }
            catch { }
        }

        private void CmdForm_SizeChanged(object sender, EventArgs e)
        {
            richTextBox1.Width = this.Width - 36;
            richTextBox1.Height = this.Height - 90;
            label1.Top = this.Height - 67;
            textBox1.Width = this.Width - 51;
            textBox1.Top = this.Height - 71;
        }
        
        private static LinkedList<string> commandHistory = new LinkedList<string>();
        private static LinkedListNode<string> commandHistoryNode = null;

        private void SetCommand(string command)
        {
            textBox1.Clear();
            textBox1.AppendText(command);
        }

        private async void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!Helper.Exited)
                {
                    string line = textBox1.Text + "\n";
                    commandHistory.AddLast(textBox1.Text);
                    commandHistoryNode = null;
                    textBox1.Clear();

                    try
                    {
                        await Helper.WriteStandardInputAsync(line);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("远端程序已退出");
                }
            }
            else if (e.KeyData == Keys.Up)
            {
                string command = "";

                if (commandHistoryNode != null)
                {
                    if (commandHistoryNode.Previous != null)
                    {
                        commandHistoryNode = commandHistoryNode.Previous;
                        command = commandHistoryNode.Value;
                    }
                    else
                    {
                        command = commandHistoryNode.Value;
                    }
                }
                else
                {
                    if (commandHistory.Last != null)
                    {
                        commandHistoryNode = commandHistory.Last;
                        command = commandHistoryNode.Value;
                    }
                }

                SetCommand(command);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == Keys.Down)
            {
                string command = "";

                if (commandHistoryNode != null)
                {
                    if (commandHistoryNode.Next != null)
                    {
                        commandHistoryNode = commandHistoryNode.Next;
                        command = commandHistoryNode.Value;
                    }
                    else
                    {
                        command = commandHistoryNode.Value;
                    }
                }

                SetCommand(command);
                e.SuppressKeyPress = true;
            }
        }
    }
}
