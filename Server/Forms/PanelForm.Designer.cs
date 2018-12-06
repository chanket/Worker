namespace Server.Forms
{
    partial class PanelForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeaderIp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderVer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderOs = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCPU = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCores = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderMem = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderGPU = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemScreenshot = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCamera = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemTaskList = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemLock = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemShutdown = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRestart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemRun = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCMD = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCMDs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemHttpDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderIp,
            this.columnHeaderName,
            this.columnHeaderVer,
            this.columnHeaderOs,
            this.columnHeaderCPU,
            this.columnHeaderCores,
            this.columnHeaderMem,
            this.columnHeaderGPU});
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(797, 379);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
            this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            // 
            // columnHeaderIp
            // 
            this.columnHeaderIp.Text = "终端";
            this.columnHeaderIp.Width = 101;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "主机名";
            this.columnHeaderName.Width = 94;
            // 
            // columnHeaderVer
            // 
            this.columnHeaderVer.Text = "版本";
            // 
            // columnHeaderOs
            // 
            this.columnHeaderOs.Text = "操作系统";
            this.columnHeaderOs.Width = 115;
            // 
            // columnHeaderCPU
            // 
            this.columnHeaderCPU.Text = "CPU";
            this.columnHeaderCPU.Width = 123;
            // 
            // columnHeaderCores
            // 
            this.columnHeaderCores.Text = "核心数";
            this.columnHeaderCores.Width = 51;
            // 
            // columnHeaderMem
            // 
            this.columnHeaderMem.Text = "物理内存";
            this.columnHeaderMem.Width = 78;
            // 
            // columnHeaderGPU
            // 
            this.columnHeaderGPU.Text = "GPU";
            this.columnHeaderGPU.Width = 131;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemScreenshot,
            this.toolStripMenuItemCamera,
            this.toolStripMenuItemTaskList,
            this.toolStripSeparator3,
            this.toolStripMenuItemLock,
            this.toolStripMenuItemShutdown,
            this.toolStripMenuItemRestart,
            this.toolStripSeparator1,
            this.toolStripMenuItemRun,
            this.toolStripMenuItemCMD,
            this.toolStripMenuItemCMDs,
            this.toolStripSeparator2,
            this.toolStripMenuItemUpload,
            this.toolStripMenuItemDownload,
            this.toolStripMenuItemHttpDownload});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(161, 286);
            // 
            // toolStripMenuItemScreenshot
            // 
            this.toolStripMenuItemScreenshot.Name = "toolStripMenuItemScreenshot";
            this.toolStripMenuItemScreenshot.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItemScreenshot.Text = "截屏";
            this.toolStripMenuItemScreenshot.Click += new System.EventHandler(this.toolStripMenuItemScreenshot_Click);
            // 
            // toolStripMenuItemCamera
            // 
            this.toolStripMenuItemCamera.Name = "toolStripMenuItemCamera";
            this.toolStripMenuItemCamera.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItemCamera.Text = "摄像头";
            this.toolStripMenuItemCamera.Click += new System.EventHandler(this.toolStripMenuItemCamera_Click);
            // 
            // toolStripMenuItemTaskList
            // 
            this.toolStripMenuItemTaskList.Name = "toolStripMenuItemTaskList";
            this.toolStripMenuItemTaskList.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItemTaskList.Text = "查看进程";
            this.toolStripMenuItemTaskList.Click += new System.EventHandler(this.toolStripMenuItemTaskList_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(157, 6);
            // 
            // toolStripMenuItemLock
            // 
            this.toolStripMenuItemLock.Name = "toolStripMenuItemLock";
            this.toolStripMenuItemLock.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItemLock.Text = "注销";
            this.toolStripMenuItemLock.Click += new System.EventHandler(this.toolStripMenuItemLock_Click);
            // 
            // toolStripMenuItemShutdown
            // 
            this.toolStripMenuItemShutdown.Name = "toolStripMenuItemShutdown";
            this.toolStripMenuItemShutdown.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItemShutdown.Text = "关机";
            this.toolStripMenuItemShutdown.Click += new System.EventHandler(this.toolStripMenuItemShutdown_Click);
            // 
            // toolStripMenuItemRestart
            // 
            this.toolStripMenuItemRestart.Name = "toolStripMenuItemRestart";
            this.toolStripMenuItemRestart.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItemRestart.Text = "重启";
            this.toolStripMenuItemRestart.Click += new System.EventHandler(this.toolStripMenuItemRestart_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // toolStripMenuItemRun
            // 
            this.toolStripMenuItemRun.Name = "toolStripMenuItemRun";
            this.toolStripMenuItemRun.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItemRun.Text = "运行";
            this.toolStripMenuItemRun.Click += new System.EventHandler(this.toolStripMenuItemRun_Click);
            // 
            // toolStripMenuItemCMD
            // 
            this.toolStripMenuItemCMD.Name = "toolStripMenuItemCMD";
            this.toolStripMenuItemCMD.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItemCMD.Text = "命令提示符";
            this.toolStripMenuItemCMD.Click += new System.EventHandler(this.toolStripMenuItemCMD_Click);
            // 
            // toolStripMenuItemCMDs
            // 
            this.toolStripMenuItemCMDs.Name = "toolStripMenuItemCMDs";
            this.toolStripMenuItemCMDs.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItemCMDs.Text = "命令提示符...";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(157, 6);
            // 
            // toolStripMenuItemUpload
            // 
            this.toolStripMenuItemUpload.Name = "toolStripMenuItemUpload";
            this.toolStripMenuItemUpload.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItemUpload.Text = "上传文件到主机";
            this.toolStripMenuItemUpload.Click += new System.EventHandler(this.toolStripMenuItemUpload_Click);
            // 
            // toolStripMenuItemDownload
            // 
            this.toolStripMenuItemDownload.Name = "toolStripMenuItemDownload";
            this.toolStripMenuItemDownload.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItemDownload.Text = "从主机下载文件";
            this.toolStripMenuItemDownload.Click += new System.EventHandler(this.toolStripMenuItemDownload_Click);
            // 
            // toolStripMenuItemHttpDownload
            // 
            this.toolStripMenuItemHttpDownload.Name = "toolStripMenuItemHttpDownload";
            this.toolStripMenuItemHttpDownload.Size = new System.Drawing.Size(160, 22);
            this.toolStripMenuItemHttpDownload.Text = "从URL下载";
            this.toolStripMenuItemHttpDownload.Click += new System.EventHandler(this.toolStripMenuItemHttpDownload_Click);
            // 
            // PanelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 403);
            this.Controls.Add(this.listView1);
            this.Name = "PanelForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "控制面板";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeaderIp;
        private System.Windows.Forms.ColumnHeader columnHeaderOs;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderCPU;
        private System.Windows.Forms.ColumnHeader columnHeaderMem;
        private System.Windows.Forms.ColumnHeader columnHeaderGPU;
        private System.Windows.Forms.ColumnHeader columnHeaderCores;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemScreenshot;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRun;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCMD;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemUpload;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLock;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDownload;
        private System.Windows.Forms.ColumnHeader columnHeaderVer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemShutdown;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRestart;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTaskList;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCMDs;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCamera;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemHttpDownload;
    }
}

