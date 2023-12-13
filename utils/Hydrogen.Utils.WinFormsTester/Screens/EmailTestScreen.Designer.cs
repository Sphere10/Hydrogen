namespace HydrogenTester.WinForms.Screens {
	partial class EmailTestScreen {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			_fromTextBox = new System.Windows.Forms.TextBox();
			label6 = new System.Windows.Forms.Label();
			_smtpPortIntBox = new Hydrogen.Windows.Forms.IntBox();
			_sslCheckBox = new System.Windows.Forms.CheckBox();
			_bodyTextBox = new System.Windows.Forms.TextBox();
			label5 = new System.Windows.Forms.Label();
			_sendButtonAsync = new System.Windows.Forms.Button();
			_sendButton = new System.Windows.Forms.Button();
			_smtpUsernameTextBox = new System.Windows.Forms.TextBox();
			label7 = new System.Windows.Forms.Label();
			_smtpPasswordTextBox = new System.Windows.Forms.TextBox();
			label8 = new System.Windows.Forms.Label();
			_toTextBox = new System.Windows.Forms.TextBox();
			_replyToTextBox = new System.Windows.Forms.TextBox();
			_smtpServerTextBox = new System.Windows.Forms.TextBox();
			_ccTextBox = new System.Windows.Forms.TextBox();
			label9 = new System.Windows.Forms.Label();
			_bccTextBox = new System.Windows.Forms.TextBox();
			label10 = new System.Windows.Forms.Label();
			label11 = new System.Windows.Forms.Label();
			_subjectTextBox = new System.Windows.Forms.TextBox();
			_isHtmlCheckBox = new System.Windows.Forms.CheckBox();
			_loadingCircle = new Hydrogen.Windows.Forms.LoadingCircle();
			SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(79, 35);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(22, 15);
			label1.TabIndex = 0;
			label1.Text = "To:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(63, 6);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(38, 15);
			label2.TabIndex = 1;
			label2.Text = "From:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(45, 122);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(56, 15);
			label3.TabIndex = 2;
			label3.Text = "Reply-To:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(26, 151);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(75, 15);
			label4.TabIndex = 3;
			label4.Text = "SMTP Server:";
			// 
			// _fromTextBox
			// 
			_fromTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_fromTextBox.Location = new System.Drawing.Point(107, 3);
			_fromTextBox.Name = "_fromTextBox";
			_fromTextBox.Size = new System.Drawing.Size(593, 23);
			_fromTextBox.TabIndex = 7;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(36, 238);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(65, 15);
			label6.TabIndex = 9;
			label6.Text = "SMTP Port:";
			// 
			// _smtpPortIntBox
			// 
			_smtpPortIntBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_smtpPortIntBox.Location = new System.Drawing.Point(107, 235);
			_smtpPortIntBox.Name = "_smtpPortIntBox";
			_smtpPortIntBox.Size = new System.Drawing.Size(593, 23);
			_smtpPortIntBox.TabIndex = 10;
			// 
			// _sslCheckBox
			// 
			_sslCheckBox.AutoSize = true;
			_sslCheckBox.Location = new System.Drawing.Point(107, 264);
			_sslCheckBox.Name = "_sslCheckBox";
			_sslCheckBox.Size = new System.Drawing.Size(44, 19);
			_sslCheckBox.TabIndex = 11;
			_sslCheckBox.Text = "SSL";
			_sslCheckBox.UseVisualStyleBackColor = true;
			// 
			// _bodyTextBox
			// 
			_bodyTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_bodyTextBox.Location = new System.Drawing.Point(107, 321);
			_bodyTextBox.MaxLength = 1048576;
			_bodyTextBox.Multiline = true;
			_bodyTextBox.Name = "_bodyTextBox";
			_bodyTextBox.Size = new System.Drawing.Size(593, 174);
			_bodyTextBox.TabIndex = 12;
			_bodyTextBox.WordWrap = false;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(64, 324);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(37, 15);
			label5.TabIndex = 13;
			label5.Text = "Body:";
			// 
			// _sendButtonAsync
			// 
			_sendButtonAsync.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			_sendButtonAsync.Location = new System.Drawing.Point(588, 501);
			_sendButtonAsync.Name = "_sendButtonAsync";
			_sendButtonAsync.Size = new System.Drawing.Size(109, 23);
			_sendButtonAsync.TabIndex = 14;
			_sendButtonAsync.Text = "Send Async";
			_sendButtonAsync.UseVisualStyleBackColor = true;
			_sendButtonAsync.Click += _sendButtonAsync_Click;
			// 
			// _sendButton
			// 
			_sendButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			_sendButton.Location = new System.Drawing.Point(473, 501);
			_sendButton.Name = "_sendButton";
			_sendButton.Size = new System.Drawing.Size(109, 23);
			_sendButton.TabIndex = 15;
			_sendButton.Text = "Send Sync";
			_sendButton.UseVisualStyleBackColor = true;
			_sendButton.Click += _sendButton_Click;
			// 
			// _smtpUsernameTextBox
			// 
			_smtpUsernameTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_smtpUsernameTextBox.Location = new System.Drawing.Point(107, 177);
			_smtpUsernameTextBox.Name = "_smtpUsernameTextBox";
			_smtpUsernameTextBox.Size = new System.Drawing.Size(593, 23);
			_smtpUsernameTextBox.TabIndex = 17;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(5, 180);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(96, 15);
			label7.TabIndex = 16;
			label7.Text = "SMTP Username:";
			// 
			// _smtpPasswordTextBox
			// 
			_smtpPasswordTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_smtpPasswordTextBox.Location = new System.Drawing.Point(107, 206);
			_smtpPasswordTextBox.Name = "_smtpPasswordTextBox";
			_smtpPasswordTextBox.Size = new System.Drawing.Size(593, 23);
			_smtpPasswordTextBox.TabIndex = 19;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(8, 209);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(93, 15);
			label8.TabIndex = 18;
			label8.Text = "SMTP Password:";
			// 
			// _toTextBox
			// 
			_toTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_toTextBox.Location = new System.Drawing.Point(107, 32);
			_toTextBox.Name = "_toTextBox";
			_toTextBox.Size = new System.Drawing.Size(593, 23);
			_toTextBox.TabIndex = 20;
			// 
			// _replyToTextBox
			// 
			_replyToTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_replyToTextBox.Location = new System.Drawing.Point(107, 119);
			_replyToTextBox.Name = "_replyToTextBox";
			_replyToTextBox.Size = new System.Drawing.Size(593, 23);
			_replyToTextBox.TabIndex = 21;
			// 
			// _smtpServerTextBox
			// 
			_smtpServerTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_smtpServerTextBox.Location = new System.Drawing.Point(107, 148);
			_smtpServerTextBox.Name = "_smtpServerTextBox";
			_smtpServerTextBox.Size = new System.Drawing.Size(593, 23);
			_smtpServerTextBox.TabIndex = 22;
			// 
			// _ccTextBox
			// 
			_ccTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_ccTextBox.Location = new System.Drawing.Point(107, 61);
			_ccTextBox.Name = "_ccTextBox";
			_ccTextBox.Size = new System.Drawing.Size(593, 23);
			_ccTextBox.TabIndex = 24;
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(75, 64);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(26, 15);
			label9.TabIndex = 23;
			label9.Text = "CC:";
			// 
			// _bccTextBox
			// 
			_bccTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_bccTextBox.Location = new System.Drawing.Point(107, 90);
			_bccTextBox.Name = "_bccTextBox";
			_bccTextBox.Size = new System.Drawing.Size(593, 23);
			_bccTextBox.TabIndex = 26;
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.Location = new System.Drawing.Point(68, 93);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(33, 15);
			label10.TabIndex = 25;
			label10.Text = "BCC:";
			// 
			// label11
			// 
			label11.AutoSize = true;
			label11.Location = new System.Drawing.Point(52, 292);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(49, 15);
			label11.TabIndex = 28;
			label11.Text = "Subject:";
			// 
			// _subjectTextBox
			// 
			_subjectTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_subjectTextBox.Location = new System.Drawing.Point(107, 289);
			_subjectTextBox.Name = "_subjectTextBox";
			_subjectTextBox.Size = new System.Drawing.Size(593, 23);
			_subjectTextBox.TabIndex = 29;
			// 
			// _isHtmlCheckBox
			// 
			_isHtmlCheckBox.AutoSize = true;
			_isHtmlCheckBox.Location = new System.Drawing.Point(157, 264);
			_isHtmlCheckBox.Name = "_isHtmlCheckBox";
			_isHtmlCheckBox.Size = new System.Drawing.Size(69, 19);
			_isHtmlCheckBox.TabIndex = 30;
			_isHtmlCheckBox.Text = "Is HTML";
			_isHtmlCheckBox.UseVisualStyleBackColor = true;
			// 
			// _loadingCircle
			// 
			_loadingCircle.Active = false;
			_loadingCircle.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			_loadingCircle.BackColor = System.Drawing.Color.Transparent;
			_loadingCircle.Color = System.Drawing.Color.DarkGray;
			_loadingCircle.HideStopControl = null;
			_loadingCircle.InnerCircleRadius = 8;
			_loadingCircle.Location = new System.Drawing.Point(437, 501);
			_loadingCircle.Name = "_loadingCircle";
			_loadingCircle.NumberSpoke = 10;
			_loadingCircle.OuterCircleRadius = 10;
			_loadingCircle.RotationSpeed = 100;
			_loadingCircle.Size = new System.Drawing.Size(30, 23);
			_loadingCircle.SpokeThickness = 4;
			_loadingCircle.TabIndex = 31;
			_loadingCircle.Text = "loadingCircle1";
			_loadingCircle.Visible = false;
			// 
			// EmailTestScreen
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add(_loadingCircle);
			Controls.Add(_isHtmlCheckBox);
			Controls.Add(_subjectTextBox);
			Controls.Add(label11);
			Controls.Add(_bccTextBox);
			Controls.Add(label10);
			Controls.Add(_ccTextBox);
			Controls.Add(label9);
			Controls.Add(_smtpServerTextBox);
			Controls.Add(_replyToTextBox);
			Controls.Add(_toTextBox);
			Controls.Add(_smtpPasswordTextBox);
			Controls.Add(label8);
			Controls.Add(_smtpUsernameTextBox);
			Controls.Add(label7);
			Controls.Add(_sendButton);
			Controls.Add(_sendButtonAsync);
			Controls.Add(label5);
			Controls.Add(_bodyTextBox);
			Controls.Add(_sslCheckBox);
			Controls.Add(_smtpPortIntBox);
			Controls.Add(label6);
			Controls.Add(_fromTextBox);
			Controls.Add(label4);
			Controls.Add(label3);
			Controls.Add(label2);
			Controls.Add(label1);
			Name = "EmailTestScreen";
			Size = new System.Drawing.Size(700, 527);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox _smtpUsernameTextBox;
		private System.Windows.Forms.TextBox _smtpPasswordTextBox;
		private System.Windows.Forms.TextBox _fromTextBox;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.Label label6;
		private Hydrogen.Windows.Forms.IntBox _smtpPortIntBox;
		private System.Windows.Forms.CheckBox _sslCheckBox;
		private System.Windows.Forms.TextBox _bodyTextBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button _sendButtonAsync;
		private System.Windows.Forms.Button _sendButton;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox _toTextBox;
		private System.Windows.Forms.TextBox _replyToTextBox;
		private System.Windows.Forms.TextBox _smtpServerTextBox;
		private System.Windows.Forms.TextBox _ccTextBox;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox _bccTextBox;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox _subjectTextBox;
		private System.Windows.Forms.CheckBox _isHtmlCheckBox;
		private Hydrogen.Windows.Forms.LoadingCircle _loadingCircle;
	}
}
