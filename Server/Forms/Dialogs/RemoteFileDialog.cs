using Common.Server.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.Forms.Dialogs
{
    public partial class RemoteFileDialog : Form
    {
        #region System
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        public enum SHGFI
        {
            SHGFI_ICON = 0x100,
            SHGFI_LARGEICON = 0x0,
            SHGFI_USEFILEATTRIBUTES = 0x10
        }

        public static Icon GetFileIcon()
        {
            SHFILEINFO _SHFILEINFO = new SHFILEINFO();
            IntPtr _IconIntPtr = SHGetFileInfo("*.idontreallycare", 0, ref _SHFILEINFO, (uint)Marshal.SizeOf(_SHFILEINFO), (uint)(SHGFI.SHGFI_ICON | SHGFI.SHGFI_LARGEICON | SHGFI.SHGFI_USEFILEATTRIBUTES));
            if (_IconIntPtr.Equals(IntPtr.Zero)) return null;
            Icon _Icon = System.Drawing.Icon.FromHandle(_SHFILEINFO.hIcon);
            return _Icon;
        }

        public static Icon GetDirectoryIcon()
        {
            SHFILEINFO _SHFILEINFO = new SHFILEINFO();
            IntPtr _IconIntPtr = SHGetFileInfo(Environment.GetEnvironmentVariable("CD"), 0, ref _SHFILEINFO, (uint)Marshal.SizeOf(_SHFILEINFO), (uint)(SHGFI.SHGFI_ICON | SHGFI.SHGFI_LARGEICON));
            if (_IconIntPtr.Equals(IntPtr.Zero)) return null;
            Icon _Icon = System.Drawing.Icon.FromHandle(_SHFILEINFO.hIcon);
            return _Icon;
        }

        public static Icon GetDriveIcon()
        {
            SHFILEINFO _SHFILEINFO = new SHFILEINFO();
            IntPtr _IconIntPtr = SHGetFileInfo(Environment.GetEnvironmentVariable("SystemDrive"), 0, ref _SHFILEINFO, (uint)Marshal.SizeOf(_SHFILEINFO), (uint)(SHGFI.SHGFI_ICON | SHGFI.SHGFI_LARGEICON | SHGFI.SHGFI_USEFILEATTRIBUTES));
            if (_IconIntPtr.Equals(IntPtr.Zero)) return null;
            Icon _Icon = System.Drawing.Icon.FromHandle(_SHFILEINFO.hIcon);
            return _Icon;
        }
        #endregion

        private ClientData Client { get; }

        private DirectoryInfo StartUp { get; } = null;

        private RemoteFileDialog()
        {
            InitializeComponent();
            listView1.SmallImageList = new ImageList();
            listView1.SmallImageList.ImageSize = new Size(16, 16);
            listView1.SmallImageList.Images.Add("file", GetFileIcon());
            listView1.SmallImageList.Images.Add("dir", GetDirectoryIcon());
            listView1.SmallImageList.Images.Add("drv", GetDriveIcon());
        }

        public RemoteFileDialog(ClientData client)
            :this()
        {
            Client = client;
        }

        private void FileDialog_Load(object sender, EventArgs e)
        {
        }

        private new void Show()
        {

        }

        private new void Show(IWin32Window owner)
        {

        }

        public new DialogResult ShowDialog()
        {
            CurrentDirectory = StartUp;
            return base.ShowDialog();
        }

        private DirectoryInfo currentDirectory;
        public DirectoryInfo CurrentDirectory
        {
            get
            {
                return currentDirectory;
            }
            protected set
            {
                SetCurrentDirectory(value);
            }
        }

        public FileSystemInfo Selected { get; set; }

        private async void SetCurrentDirectory(DirectoryInfo value)
        {
            Selected = null;
            listView1.Clear();
            listView1.BeginUpdate();

            using (FileSystemHelper helper = new FileSystemHelper(Client))
            {
                try
                {
                    if (value != null)
                    {
                        foreach (var di in await helper.GetDirectoriesAsync(value))
                        {
                            if (!this.IsDisposed)
                            {
                                listView1.Items.Add(new ListViewItem(di.Name)
                                {
                                    Tag = di,
                                    ImageKey = "dir",
                                });
                            }
                        }
                        foreach (var fi in await helper.GetFilesAsync(value))
                        {
                            if (!this.IsDisposed)
                            {
                                listView1.Items.Add(new ListViewItem(fi.Name)
                                {
                                    Tag = fi,
                                    ImageKey = "file",
                                });
                            }
                        }

                        this.Text = value.FullName;
                    }
                    else
                    {
                        foreach (var di in await helper.GetDrivesAsync())
                        {
                            if (!this.IsDisposed)
                            {
                                listView1.Items.Add(new ListViewItem(di.Name)
                                {
                                    Tag = di,
                                    ImageKey = "drv",
                                });
                            }
                        }

                        this.Text = "选择文件";
                    }

                    currentDirectory = value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    //CurrentDirectory = currentDirectory;
                    Close();
                }
            }

            listView1.EndUpdate();
        }

        private void FileDialog_SizeChanged(object sender, EventArgs e)
        {
            listView1.Width = this.Width - 121;
            listView1.Height = this.Height - 90;
            button1.Top = this.Height - 80;
            button1.Left = this.Width - 103;
            button3.Left = this.Width - 103;
            button4.Left = this.Width - 103;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var info = listView1.SelectedItems[0].Tag;
                if (info is DirectoryInfo)
                {
                    this.CurrentDirectory = info as DirectoryInfo;
                }
                else if (info is DriveInfo)
                {
                    this.CurrentDirectory = (info as DriveInfo).RootDirectory;
                }
            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var info = listView1.SelectedItems[0].Tag;
                if (info is DriveInfo)
                {
                    Selected = (info as DriveInfo).RootDirectory;
                }
                else if (info is FileSystemInfo)
                {
                    Selected = info as FileSystemInfo;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var info = listView1.SelectedItems[0].Tag;
                if (info is FileInfo)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else if (info is DirectoryInfo)
                {
                    this.CurrentDirectory = info as DirectoryInfo;
                }
                else if (info is DriveInfo)
                {
                    this.CurrentDirectory = (info as DriveInfo).RootDirectory;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CurrentDirectory = null;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var cd = CurrentDirectory;
            if (cd != null)
            {
                CurrentDirectory = cd.Parent;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
