using Common.Frames.TaskList;
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
    public partial class TaskListForm : Form
    {
        private static string Filter { get; set; } = "";

        private ClientData Client { get; }

        private List<TaskInfo> Tasks { get; set; } = new List<TaskInfo>();

        private TaskListForm()
        {
            InitializeComponent();
        }

        public TaskListForm(ClientData client)
            : this()
        {
            Client = client;
        }

        private async Task UpdateTasks()
        {
            Tasks = await GetTasks();
            if (!listView1.IsDisposed) DisplayTasks(Tasks, textBox1.Text);
        }

        private async Task<List<TaskInfo>> GetTasks()
        {
            try
            {
                using (TaskListHelper helper = new TaskListHelper(Client))
                {
                    return await helper.GetAsync();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new List<TaskInfo>();
            }
        }

        private void DisplayTasks(List<TaskInfo> tasks, string filter)
        {
            listView1.BeginUpdate();
            {
                listView1.Items.Clear();
                foreach (var task in Tasks)
                {
                    if (string.IsNullOrEmpty(filter) || task.Name.ToLower().Contains(filter.ToLower()))
                    {
                        ListViewItem lvi = new ListViewItem(new string[] {
                                task.Name,
                                task.Pid.ToString(),
                                (task.Memory / 1024).ToString() + " KB",
                                task.Path
                            });
                        lvi.Tag = new int?(task.Pid);
                        listView1.Items.Add(lvi);
                    }
                }
            }
            listView1.EndUpdate();
        }

        private async void TaskListForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = Filter;
            textBox1.Focus();
            await UpdateTasks();
        }

        private async void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F5:
                    {
                        await UpdateTasks();
                    }
                    break;

                case Keys.Delete:
                    {
                        if (listView1.SelectedItems.Count > 0 && MessageBox.Show("Are your sure to terminate the task?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            try
                            {
                                using (TaskListHelper helper = new TaskListHelper(Client))
                                {
                                    await helper.StopAsync((listView1.SelectedItems[0].Tag as int?).Value);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            await Task.Delay(1);
                            await UpdateTasks();
                        }
                    }
                    break;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DisplayTasks(Tasks, textBox1.Text);
            Filter = textBox1.Text;
        }

        private void TaskListForm_SizeChanged(object sender, EventArgs e)
        {
            listView1.Width = this.Width - 40;
            listView1.Height = this.Height - 90;
            textBox1.Width = this.Width - 117;
        }
    }
}
