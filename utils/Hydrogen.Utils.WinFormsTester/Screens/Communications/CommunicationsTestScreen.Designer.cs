//-----------------------------------------------------------------------
// <copyright file="ScreenA.Designer.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
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
            this.Reset = new System.Windows.Forms.Button();
            this.Report = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.Clients = new System.Windows.Forms.DataGridView();
            this.ColumnId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnConnected = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnClose = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.SendPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Clients)).BeginInit();
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
            this.Output.Size = new System.Drawing.Size(1281, 393);
            this.Output.TabIndex = 0;
            // 
            // Send
            // 
            this.Send.Location = new System.Drawing.Point(4, 91);
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
            this.Protocol.Location = new System.Drawing.Point(59, 227);
            this.Protocol.Name = "Protocol";
            this.Protocol.Size = new System.Drawing.Size(87, 49);
            this.Protocol.TabIndex = 5;
            this.Protocol.SelectedValueChanged += new System.EventHandler(this.Protocol_SelectedValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 227);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Protocol";
            // 
            // Message
            // 
            this.Message.Location = new System.Drawing.Point(154, 228);
            this.Message.Multiline = true;
            this.Message.Name = "Message";
            this.Message.Size = new System.Drawing.Size(172, 48);
            this.Message.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(155, 211);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Message";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(145, 163);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "Send IP";
            // 
            // SendIP
            // 
            this.SendIP.Location = new System.Drawing.Point(189, 160);
            this.SendIP.Name = "SendIP";
            this.SendIP.Size = new System.Drawing.Size(78, 23);
            this.SendIP.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 163);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "Send Port";
            // 
            // SendPort
            // 
            this.SendPort.Location = new System.Drawing.Point(69, 161);
            this.SendPort.Name = "SendPort";
            this.SendPort.Size = new System.Drawing.Size(70, 23);
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
            this.MyIPs.Location = new System.Drawing.Point(75, 3);
            this.MyIPs.Name = "MyIPs";
            this.MyIPs.Size = new System.Drawing.Size(254, 117);
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
            this.label1.Location = new System.Drawing.Point(4, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "My IP";
            // 
            // MyIp
            // 
            this.MyIp.Location = new System.Drawing.Point(69, 132);
            this.MyIp.Name = "MyIp";
            this.MyIp.Size = new System.Drawing.Size(68, 23);
            this.MyIp.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(143, 135);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 15);
            this.label7.TabIndex = 2;
            this.label7.Text = "Role";
            // 
            // Role
            // 
            this.Role.Location = new System.Drawing.Point(189, 132);
            this.Role.Name = "Role";
            this.Role.Size = new System.Drawing.Size(78, 23);
            this.Role.TabIndex = 8;
            // 
            // CloseConnection
            // 
            this.CloseConnection.Location = new System.Drawing.Point(282, 132);
            this.CloseConnection.Name = "CloseConnection";
            this.CloseConnection.Size = new System.Drawing.Size(44, 24);
            this.CloseConnection.TabIndex = 1;
            this.CloseConnection.Text = "Close";
            this.CloseConnection.UseVisualStyleBackColor = true;
            this.CloseConnection.Click += new System.EventHandler(this.CloseConnection_Click);
            // 
            // Reset
            // 
            this.Reset.Location = new System.Drawing.Point(282, 160);
            this.Reset.Name = "Reset";
            this.Reset.Size = new System.Drawing.Size(44, 24);
            this.Reset.TabIndex = 1;
            this.Reset.Text = "Reset";
            this.Reset.UseVisualStyleBackColor = true;
            this.Reset.Click += new System.EventHandler(this.Reset_Click);
            // 
            // Report
            // 
            this.Report.Location = new System.Drawing.Point(69, 189);
            this.Report.Name = "Report";
            this.Report.Size = new System.Drawing.Size(257, 23);
            this.Report.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 192);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 15);
            this.label8.TabIndex = 6;
            this.label8.Text = "Report";
            // 
            // Clients
            // 
            this.Clients.AllowUserToAddRows = false;
            this.Clients.AllowUserToDeleteRows = false;
            this.Clients.AllowUserToResizeColumns = false;
            this.Clients.AllowUserToResizeRows = false;
            this.Clients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Clients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnId,
            this.ColumnStatus,
            this.ColumnConnected,
            this.ColumnClose});
            this.Clients.Location = new System.Drawing.Point(332, 3);
            this.Clients.MultiSelect = false;
            this.Clients.Name = "Clients";
            this.Clients.RowHeadersVisible = false;
            this.Clients.RowTemplate.Height = 25;
            this.Clients.Size = new System.Drawing.Size(509, 273);
            this.Clients.TabIndex = 15;
            this.Clients.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Clients_CellContentClick);
            // 
            // ColumnId
            // 
            this.ColumnId.HeaderText = "Id";
            this.ColumnId.Name = "ColumnId";
            this.ColumnId.ReadOnly = true;
            this.ColumnId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnId.Width = 250;
            // 
            // ColumnStatus
            // 
            this.ColumnStatus.HeaderText = "Status";
            this.ColumnStatus.Name = "ColumnStatus";
            this.ColumnStatus.ReadOnly = true;
            this.ColumnStatus.Width = 60;
            // 
            // ColumnConnected
            // 
            this.ColumnConnected.HeaderText = "Connected";
            this.ColumnConnected.Name = "ColumnConnected";
            this.ColumnConnected.ReadOnly = true;
            this.ColumnConnected.Width = 70;
            // 
            // ColumnClose
            // 
            this.ColumnClose.HeaderText = "Action";
            this.ColumnClose.Name = "ColumnClose";
            this.ColumnClose.ReadOnly = true;
            this.ColumnClose.Width = 50;
            // 
            // CommunicationsTestScreen
            // 
            this.Controls.Add(this.Clients);
            this.Controls.Add(this.Report);
            this.Controls.Add(this.MyIPs);
            this.Controls.Add(this.Role);
            this.Controls.Add(this.MyIp);
            this.Controls.Add(this.SendIP);
            this.Controls.Add(this.Message);
            this.Controls.Add(this.label8);
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
            this.Controls.Add(this.CloseConnection);
            this.Controls.Add(this.Send);
            this.Controls.Add(this.Output);
            this.Name = "CommunicationsTestScreen";
            this.Size = new System.Drawing.Size(1312, 693);
            this.Load += new System.EventHandler(this.WebSocketsScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SendPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Clients)).EndInit();
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
		private System.Windows.Forms.Button Reset;
		private System.Windows.Forms.TextBox Report;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.DataGridView Clients;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnId;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnStatus;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnConnected;
		private System.Windows.Forms.DataGridViewButtonColumn ColumnClose;
	}
}
