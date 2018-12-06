namespace Server.Forms
{
    partial class UploadForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxRemote = new System.Windows.Forms.TextBox();
            this.textBoxLocal = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeaderEp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderProcess = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timerSpeed = new System.Windows.Forms.Timer(this.components);
            this.timerProgress = new System.Windows.Forms.Timer(this.components);
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 287);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(383, 36);
            this.button1.TabIndex = 15;
            this.button1.Text = "上传";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "远程目录";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "本地文件";
            // 
            // textBoxRemote
            // 
            this.textBoxRemote.Location = new System.Drawing.Point(14, 63);
            this.textBoxRemote.Name = "textBoxRemote";
            this.textBoxRemote.Size = new System.Drawing.Size(383, 21);
            this.textBoxRemote.TabIndex = 12;
            this.textBoxRemote.Text = "%WINDIR%\\System32";
            // 
            // textBoxLocal
            // 
            this.textBoxLocal.Location = new System.Drawing.Point(14, 24);
            this.textBoxLocal.Name = "textBoxLocal";
            this.textBoxLocal.ReadOnly = true;
            this.textBoxLocal.Size = new System.Drawing.Size(329, 21);
            this.textBoxLocal.TabIndex = 11;
            this.textBoxLocal.TextChanged += new System.EventHandler(this.textBoxLocal_TextChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(349, 23);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(48, 23);
            this.button2.TabIndex = 16;
            this.button2.Text = "Open";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderEp,
            this.columnHeaderProcess,
            this.columnHeaderStatus});
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(14, 90);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(383, 169);
            this.listView1.TabIndex = 18;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderEp
            // 
            this.columnHeaderEp.Text = "终端";
            this.columnHeaderEp.Width = 185;
            // 
            // columnHeaderProcess
            // 
            this.columnHeaderProcess.Text = "进度";
            this.columnHeaderProcess.Width = 58;
            // 
            // columnHeaderStatus
            // 
            this.columnHeaderStatus.Text = "状态";
            this.columnHeaderStatus.Width = 113;
            // 
            // timerSpeed
            // 
            this.timerSpeed.Interval = 500;
            this.timerSpeed.Tick += new System.EventHandler(this.timerSpeed_Tick);
            // 
            // timerProgress
            // 
            this.timerProgress.Tick += new System.EventHandler(this.timerProgress_Tick);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(14, 265);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(84, 16);
            this.checkBox1.TabIndex = 19;
            this.checkBox1.Text = "完成后运行";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // UploadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 335);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxRemote);
            this.Controls.Add(this.textBoxLocal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "UploadForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "上传";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.UploadForm_FormClosed);
            this.Load += new System.EventHandler(this.UploadForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxRemote;
        private System.Windows.Forms.TextBox textBoxLocal;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeaderEp;
        private System.Windows.Forms.ColumnHeader columnHeaderProcess;
        private System.Windows.Forms.ColumnHeader columnHeaderStatus;
        private System.Windows.Forms.Timer timerSpeed;
        private System.Windows.Forms.Timer timerProgress;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}