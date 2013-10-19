namespace UdpMessageTest
{
	partial class UdpMessageTest
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
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox_ip = new System.Windows.Forms.TextBox();
			this.label_ip = new System.Windows.Forms.Label();
			this.textBox_port = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button_connect = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.button_login = new System.Windows.Forms.Button();
			this.button_clearReceived = new System.Windows.Forms.Button();
			this.comboBox = new System.Windows.Forms.ComboBox();
			this.button_send = new System.Windows.Forms.Button();
			this.button_close = new System.Windows.Forms.Button();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.richTextBox_Send = new System.Windows.Forms.RichTextBox();
			this.richTextBox_received = new System.Windows.Forms.RichTextBox();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBox_ip
			// 
			this.textBox_ip.Location = new System.Drawing.Point(41, 12);
			this.textBox_ip.Name = "textBox_ip";
			this.textBox_ip.Size = new System.Drawing.Size(88, 21);
			this.textBox_ip.TabIndex = 0;
			this.textBox_ip.Text = "127.0.0.1";
			// 
			// label_ip
			// 
			this.label_ip.AutoSize = true;
			this.label_ip.Location = new System.Drawing.Point(12, 15);
			this.label_ip.Name = "label_ip";
			this.label_ip.Size = new System.Drawing.Size(23, 12);
			this.label_ip.TabIndex = 1;
			this.label_ip.Text = "IP:";
			// 
			// textBox_port
			// 
			this.textBox_port.Location = new System.Drawing.Point(184, 12);
			this.textBox_port.Name = "textBox_port";
			this.textBox_port.Size = new System.Drawing.Size(62, 21);
			this.textBox_port.TabIndex = 0;
			this.textBox_port.Text = "2001";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(143, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "Port:";
			// 
			// button_connect
			// 
			this.button_connect.Location = new System.Drawing.Point(252, 12);
			this.button_connect.Name = "button_connect";
			this.button_connect.Size = new System.Drawing.Size(75, 23);
			this.button_connect.TabIndex = 2;
			this.button_connect.Text = "连接";
			this.button_connect.UseVisualStyleBackColor = true;
			this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.button_login);
			this.panel1.Controls.Add(this.button_clearReceived);
			this.panel1.Controls.Add(this.comboBox);
			this.panel1.Controls.Add(this.button_send);
			this.panel1.Controls.Add(this.button_close);
			this.panel1.Controls.Add(this.button_connect);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.textBox_port);
			this.panel1.Controls.Add(this.label_ip);
			this.panel1.Controls.Add(this.textBox_ip);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(776, 41);
			this.panel1.TabIndex = 0;
			// 
			// button_login
			// 
			this.button_login.Location = new System.Drawing.Point(689, 12);
			this.button_login.Name = "button_login";
			this.button_login.Size = new System.Drawing.Size(75, 23);
			this.button_login.TabIndex = 6;
			this.button_login.Text = "登录";
			this.button_login.UseVisualStyleBackColor = true;
			this.button_login.Click += new System.EventHandler(this.button_login_Click);
			// 
			// button_clearReceived
			// 
			this.button_clearReceived.Location = new System.Drawing.Point(414, 12);
			this.button_clearReceived.Name = "button_clearReceived";
			this.button_clearReceived.Size = new System.Drawing.Size(63, 23);
			this.button_clearReceived.TabIndex = 5;
			this.button_clearReceived.Text = "清空";
			this.button_clearReceived.UseVisualStyleBackColor = true;
			this.button_clearReceived.Click += new System.EventHandler(this.button_clearReceived_Click);
			// 
			// comboBox
			// 
			this.comboBox.FormattingEnabled = true;
			this.comboBox.Items.AddRange(new object[] {
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13"});
			this.comboBox.Location = new System.Drawing.Point(483, 14);
			this.comboBox.Name = "comboBox";
			this.comboBox.Size = new System.Drawing.Size(121, 20);
			this.comboBox.TabIndex = 4;
			this.comboBox.Text = "08";
			// 
			// button_send
			// 
			this.button_send.Enabled = false;
			this.button_send.Location = new System.Drawing.Point(610, 12);
			this.button_send.Name = "button_send";
			this.button_send.Size = new System.Drawing.Size(75, 23);
			this.button_send.TabIndex = 3;
			this.button_send.Text = "发送";
			this.button_send.UseVisualStyleBackColor = true;
			this.button_send.Click += new System.EventHandler(this.button_send_Click);
			// 
			// button_close
			// 
			this.button_close.Enabled = false;
			this.button_close.Location = new System.Drawing.Point(333, 12);
			this.button_close.Name = "button_close";
			this.button_close.Size = new System.Drawing.Size(75, 23);
			this.button_close.TabIndex = 3;
			this.button_close.Text = "断开";
			this.button_close.UseVisualStyleBackColor = true;
			this.button_close.Click += new System.EventHandler(this.button_close_Click);
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 41);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.richTextBox_Send);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.richTextBox_received);
			this.splitContainer.Size = new System.Drawing.Size(776, 479);
			this.splitContainer.SplitterDistance = 100;
			this.splitContainer.TabIndex = 1;
			// 
			// richTextBox_Send
			// 
			this.richTextBox_Send.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox_Send.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox_Send.Location = new System.Drawing.Point(0, 0);
			this.richTextBox_Send.Name = "richTextBox_Send";
			this.richTextBox_Send.Size = new System.Drawing.Size(776, 100);
			this.richTextBox_Send.TabIndex = 0;
			this.richTextBox_Send.Text = "";
			// 
			// richTextBox_received
			// 
			this.richTextBox_received.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox_received.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox_received.Location = new System.Drawing.Point(0, 0);
			this.richTextBox_received.Name = "richTextBox_received";
			this.richTextBox_received.Size = new System.Drawing.Size(776, 375);
			this.richTextBox_received.TabIndex = 0;
			this.richTextBox_received.Text = "";
			// 
			// UdpMessageTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(776, 520);
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.panel1);
			this.Name = "UdpMessageTest";
			this.Text = "UDP消息测试";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UdpMessageTest_FormClosing);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox textBox_ip;
		private System.Windows.Forms.Label label_ip;
		private System.Windows.Forms.TextBox textBox_port;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button_connect;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.RichTextBox richTextBox_received;
		private System.Windows.Forms.RichTextBox richTextBox_Send;
		private System.Windows.Forms.Button button_send;
		private System.Windows.Forms.Button button_close;
		private System.Windows.Forms.ComboBox comboBox;
		private System.Windows.Forms.Button button_clearReceived;
		private System.Windows.Forms.Button button_login;

	}
}

