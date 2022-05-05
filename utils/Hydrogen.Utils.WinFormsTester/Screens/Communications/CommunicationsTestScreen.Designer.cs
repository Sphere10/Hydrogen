//-----------------------------------------------------------------------
// <copyright file="ScreenA.Designer.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

namespace Hydrogen.Utils.WinFormsTester
{
	partial class CommunicationsTestScreen
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
			this.Output = new Hydrogen.Windows.Forms.TextBoxEx();
			this.Send = new System.Windows.Forms.Button();
			this.Protocol = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.Message = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.SendIP = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.SendPort = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.MyIPs = new System.Windows.Forms.ListView();
			this.IP = new System.Windows.Forms.ColumnHeader();
			this.Type = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.MyIp = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.Role = new System.Windows.Forms.TextBox();
			this.CloseConnection = new System.Windows.Forms.Button();
			this.Info = new System.Windows.Forms.Button();
			this.Reset = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.SendPort)).BeginInit();
			this.SuspendLayout();
			// 
			// Output
			// 
			this.Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Output.Location = new System.Drawing.Point(13, 283);
			this.Output.Multiline = true;
			this.Output.Name = "Output";
			this.Output.PlaceholderText = "Output";
			this.Output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.Output.Size = new System.Drawing.Size(880, 306);
			this.Output.TabIndex = 0;
			// 
			// Send
			// 
			this.Send.Location = new System.Drawing.Point(13, 141);
			this.Send.Name = "Send";
			this.Send.Size = new System.Drawing.Size(56, 29);
			this.Send.TabIndex = 1;
			this.Send.Text = "Send";
			this.Send.UseVisualStyleBackColor = true;
			this.Send.Click += new System.EventHandler(this.Send_Click);
			// 
			// Protocol
			// 
			this.Protocol.FormattingEnabled = true;
			this.Protocol.ItemHeight = 15;
			this.Protocol.Items.AddRange(new object[] {
            "UDP",
            "TCP",
            "WebSockets"});
			this.Protocol.Location = new System.Drawing.Point(137, 219);
			this.Protocol.Name = "Protocol";
			this.Protocol.Size = new System.Drawing.Size(87, 49);
			this.Protocol.TabIndex = 5;
			this.Protocol.SelectedValueChanged += new System.EventHandler(this.Protocol_SelectedValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(79, 219);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(52, 15);
			this.label2.TabIndex = 6;
			this.label2.Text = "Protocol";
			// 
			// Message
			// 
			this.Message.Location = new System.Drawing.Point(245, 219);
			this.Message.Multiline = true;
			this.Message.Name = "Message";
			this.Message.Size = new System.Drawing.Size(172, 48);
			this.Message.TabIndex = 7;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(245, 201);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 15);
			this.label3.TabIndex = 6;
			this.label3.Text = "Message";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(243, 172);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(46, 15);
			this.label4.TabIndex = 2;
			this.label4.Text = "Send IP";
			// 
			// SendIP
			// 
			this.SendIP.Location = new System.Drawing.Point(303, 169);
			this.SendIP.Name = "SendIP";
			this.SendIP.Size = new System.Drawing.Size(87, 23);
			this.SendIP.TabIndex = 8;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(83, 172);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(58, 15);
			this.label5.TabIndex = 2;
			this.label5.Text = "Send Port";
			// 
			// SendPort
			// 
			this.SendPort.Location = new System.Drawing.Point(147, 170);
			this.SendPort.Name = "SendPort";
			this.SendPort.Size = new System.Drawing.Size(87, 23);
			this.SendPort.TabIndex = 3;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(18, 14);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(42, 15);
			this.label6.TabIndex = 2;
			this.label6.Text = "My IPs";
			// 
			// MyIPs
			// 
			this.MyIPs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IP,
            this.Type});
			this.MyIPs.HideSelection = false;
			this.MyIPs.Location = new System.Drawing.Point(75, 3);
			this.MyIPs.Name = "MyIPs";
			this.MyIPs.Size = new System.Drawing.Size(342, 117);
			this.MyIPs.TabIndex = 10;
			this.MyIPs.UseCompatibleStateImageBehavior = false;
			this.MyIPs.View = System.Windows.Forms.View.Details;
			// 
			// IP
			// 
			this.IP.Width = 100;
			// 
			// Type
			// 
			this.Type.Width = 150;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(82, 144);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 15);
			this.label1.TabIndex = 2;
			this.label1.Text = "My IP";
			// 
			// MyIp
			// 
			this.MyIp.Location = new System.Drawing.Point(147, 141);
			this.MyIp.Name = "MyIp";
			this.MyIp.Size = new System.Drawing.Size(87, 23);
			this.MyIp.TabIndex = 8;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(241, 144);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(30, 15);
			this.label7.TabIndex = 2;
			this.label7.Text = "Role";
			// 
			// Role
			// 
			this.Role.Location = new System.Drawing.Point(303, 141);
			this.Role.Name = "Role";
			this.Role.Size = new System.Drawing.Size(87, 23);
			this.Role.TabIndex = 8;
			// 
			// CloseConnection
			// 
			this.CloseConnection.Location = new System.Drawing.Point(456, 141);
			this.CloseConnection.Name = "CloseConnection";
			this.CloseConnection.Size = new System.Drawing.Size(56, 29);
			this.CloseConnection.TabIndex = 1;
			this.CloseConnection.Text = "Close";
			this.CloseConnection.UseVisualStyleBackColor = true;
			this.CloseConnection.Click += new System.EventHandler(this.CloseConnection_Click);
			// 
			// Info
			// 
			this.Info.Location = new System.Drawing.Point(456, 172);
			this.Info.Name = "Info";
			this.Info.Size = new System.Drawing.Size(56, 29);
			this.Info.TabIndex = 1;
			this.Info.Text = "Info";
			this.Info.UseVisualStyleBackColor = true;
			this.Info.Click += new System.EventHandler(this.Info_Click);
			// 
			// Reset
			// 
			this.Reset.Location = new System.Drawing.Point(456, 203);
			this.Reset.Name = "Reset";
			this.Reset.Size = new System.Drawing.Size(56, 29);
			this.Reset.TabIndex = 1;
			this.Reset.Text = "Reset";
			this.Reset.UseVisualStyleBackColor = true;
			this.Reset.Click += new System.EventHandler(this.Reset_Click);
			// 
			// CommunicationsTestScreen
			// 
			this.Controls.Add(this.MyIPs);
			this.Controls.Add(this.Role);
			this.Controls.Add(this.MyIp);
			this.Controls.Add(this.SendIP);
			this.Controls.Add(this.Message);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.Protocol);
			this.Controls.Add(this.SendPort);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.Reset);
			this.Controls.Add(this.Info);
			this.Controls.Add(this.CloseConnection);
			this.Controls.Add(this.Send);
			this.Controls.Add(this.Output);
			this.Name = "CommunicationsTestScreen";
			this.Size = new System.Drawing.Size(911, 606);
			this.Load += new System.EventHandler(this.WebSocketsScreen_Load);
			((System.ComponentModel.ISupportInitialize)(this.SendPort)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Hydrogen.Windows.Forms.TextBoxEx Output;
		private System.Windows.Forms.Button Send;
		private System.Windows.Forms.ListBox Protocol;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox Message;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox SendIP;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown SendPort;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ListView MyIPs;
		private System.Windows.Forms.ColumnHeader IP;
		private System.Windows.Forms.ColumnHeader Type;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox MyIp;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox Role;
		private System.Windows.Forms.Button CloseConnection;
		private System.Windows.Forms.Button Info;
		private System.Windows.Forms.Button Reset;
	}
}
