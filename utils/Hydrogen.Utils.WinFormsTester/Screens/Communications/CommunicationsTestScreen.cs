// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Linq;
using Hydrogen.Communications;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class CommunicationsTestScreen : ApplicationScreen {
	const int NodeDiscoveryPort = 21000;
	CommunicationRole CommunicationRole { get; set; }

	UDPChannel UDPChannel { get; set; }

	TCPChannel TCPChannel { get; set; }

	//WebSocketsChannel  WebSocketsChannel { get; set; }
	ServerWebSocketsChannel ServerWebSocketsChannel { get; set; }
	ClientWebSocketsChannel ClientWebSocketsChannel { get; set; }

	System.Timers.Timer Timer { get; set; }

	private ServerWebSocketsDataSource<TestClass> DataSource { get; set; }

	public CommunicationsTestScreen() {
		InitializeComponent();
		SystemLog.RegisterLogger(new TextBoxLogger(Output));
		SystemLog.RegisterLogger(new FileAppendLogger("c:/temp/test.txt", true));
	}

	private void WebSocketsScreen_Load(object sender, EventArgs e) {
		SendPort.Value = 80;
		var localHost = Dns.GetHostEntry(Dns.GetHostName());
		foreach (var ipAddress in localHost.AddressList) {
			if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6) continue;

			var ipString = ipAddress.ToString();
			var addressFamiliy = ipAddress.AddressFamily;

			MyIPs.Items.Add(ipString).SubItems.Add(addressFamiliy.ToString());

			if (ipString.Contains("192.168")) {
				MyIp.Text = ipString;
			}
		}

		if (MyIp.Text == "192.168.1.108") // virtual computer
		{
			SendIP.Text = "192.168.1.171"; // "192.168.1.106";
			Message.Text = "Message from TWO";
			CommunicationRole = CommunicationRole.Client;
			//CommunicationRole = CommunicationRole.Server;
		} else // Actual Desktop
		{
			SendIP.Text = "192.168.1.108";
			Message.Text = "Message from ONE";
			CommunicationRole = CommunicationRole.Server;
			//CommunicationRole = CommunicationRole.Client;
		}

		Role.Text = CommunicationRole.ToString();
		Protocol.Text = "WebSockets"; // default on TCP, will run Protocol_SelectedValueChanged()

		var localEndpoint = new IPEndPoint(IPAddress.Parse(MyIp.Text), (int)SendPort.Value);
		var remoteEndpoint = new IPEndPoint(IPAddress.Parse(SendIP.Text), (int)SendPort.Value);
		DataSource = new ServerWebSocketsDataSource<TestClass>(localEndpoint, remoteEndpoint, false, InitializeItem, UpdateItem, IdItem);

		Timer = new System.Timers.Timer(1000); // 1 seconds
		Timer.Elapsed += Timer_Elapsed;
		Timer.Start();
	}

	string PreviousReport { get; set; }
	private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {

		if (DataSource == null)
			return;

		Invoke((MethodInvoker)delegate { DoReport(); });
	}

	void DoReport() {
		var report = DataSource.Report();
		var lines = report.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
		if (lines.Count < 1)
			return;

		int reportId = -1;
		int.TryParse(lines.Last().Replace("ReportId:", ""), out reportId);
		Report.Text = reportId.ToString();

		// remove the report id line, the last line
		lines.RemoveAt(lines.Count - 1);
		report = String.Join("\r\n", lines);

		if (PreviousReport == report) {
			Report.Text += " Not Updated";
			return;
		}

		Report.Text += " Updated";

		PreviousReport = report;

		Clients.Rows.Clear();
		for (int i = 0; i < lines.Count; i++) {
			var tokens = lines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

			var id = tokens[1];
			var state = tokens[3];
			var connected = tokens[5];

			var row = new string[] { id, state, connected, "Close" };
			int index = Clients.Rows.Add(row);

			var stateColor = System.Drawing.Color.Black;
			switch (state) {
				case "Opening":
					stateColor = System.Drawing.Color.Blue;
					break;
				case "Open":
					stateColor = System.Drawing.Color.Green;
					break;
				case "Closing":
					stateColor = System.Drawing.Color.Orange;
					break;
				case "Closed":
					stateColor = System.Drawing.Color.Red;
					break;
			}
			Clients.Rows[index].Cells[1].Style.ForeColor = stateColor;

			var connectedColor = connected == "True" ? System.Drawing.Color.Green : System.Drawing.Color.Red;
			Clients.Rows[index].Cells[2].Style.ForeColor = connectedColor;
		}
	}

	string InitializeItem(TestClass item, int id) {
		try {
			item.FillWithTestData(id);
		} catch (Exception ex) {
			return ex.Message;
		}

		return null;
	}

	string UpdateItem(TestClass item) {
		try {
			// do something here

		} catch (Exception ex) {
			return ex.Message;
		}

		return null;
	}

	string IdItem(TestClass item) {
		return item.Id.ToString();
	}

	private void Send_Click(object sender, EventArgs e) {
		var data = Encoding.ASCII.GetBytes(Message.Text);

		if (Protocol.Text == "UDP" && UDPChannel != null) {
			UDPChannel.TrySendBytes(data);
		} else if (Protocol.Text == "TCP" && TCPChannel != null) {
			TCPChannel.TrySendBytes(data);
		} else if (Protocol.Text == "WebSockets") {
			if (CommunicationRole == CommunicationRole.Server) {
				ServerWebSocketsChannel?.TrySendBytes(data);
			} else {
				ClientWebSocketsChannel?.TrySendBytes(data);
			}
		}
	}

	async Task Close() {
		if (DataSource != null) {
			DataSource.Close();
			DataSource = null;
			return;
		}

		if (UDPChannel != null) {
			await UDPChannel.Close();
			UDPChannel = null;
		}

		if (TCPChannel != null) {
			await TCPChannel.Close();
			TCPChannel = null;
		}

		if (CommunicationRole == CommunicationRole.Server && ServerWebSocketsChannel != null) {
			await ServerWebSocketsChannel.Close();
			ServerWebSocketsChannel = null;
		} else if (CommunicationRole == CommunicationRole.Client && ClientWebSocketsChannel != null) {
			await ClientWebSocketsChannel.Close();
			ClientWebSocketsChannel = null;
		}
	}

	async void Setup() {
		var localEndpoint = new IPEndPoint(IPAddress.Parse(MyIp.Text), (int)SendPort.Value);
		var remoteEndpoint = new IPEndPoint(IPAddress.Parse(SendIP.Text), (int)SendPort.Value);

		if (Protocol.Text == "UDP") {
			UDPChannel = new UDPChannel(localEndpoint, remoteEndpoint, CommunicationRole.Client);
			UDPChannel.ReceivedBytes += delegate(ReadOnlyMemory<byte> memory) { Output.Invoke((MethodInvoker)delegate { Output.AppendLine("UDP: " + Encoding.ASCII.GetString(memory.Span)); }); };
			await UDPChannel.Open();
		} else if (Protocol.Text == "TCP") {
			TCPChannel = new TCPChannel(localEndpoint, remoteEndpoint, CommunicationRole.Client);
			TCPChannel.ReceivedBytes += delegate(ReadOnlyMemory<byte> memory) { Output.Invoke((MethodInvoker)delegate { Output.AppendLine("TCP: " + Encoding.ASCII.GetString(memory.Span)); }); };
			await TCPChannel.Open();
		} else if (Protocol.Text == "WebSockets") {
			var secure = false;
			var socketType = secure ? "wss" : "ws";
			if (CommunicationRole == CommunicationRole.Server) {
				ServerWebSocketsChannel = new ServerWebSocketsChannel(localEndpoint, remoteEndpoint, secure);
				ServerWebSocketsChannel.ReceivedBytes += delegate(ReadOnlyMemory<byte> memory) { Output.Invoke((MethodInvoker)delegate { Output.AppendLine("WebSockets: " + Encoding.ASCII.GetString(memory.Span)); }); };
				await ServerWebSocketsChannel.Open();
			} else // client
			{
				ClientWebSocketsChannel = new ClientWebSocketsChannel($"{socketType}://{remoteEndpoint.Address}:{remoteEndpoint.Port}", secure);
				ClientWebSocketsChannel.ReceivedBytes += delegate(ReadOnlyMemory<byte> memory) { Output.Invoke((MethodInvoker)delegate { Output.AppendLine("WebSockets: " + Encoding.ASCII.GetString(memory.Span)); }); };
				await ClientWebSocketsChannel.Open();
			}
		}
	}

	async private void Protocol_SelectedValueChanged(object sender, EventArgs e) {
		Output.AppendLine("---------------------------------------------");
		await Close();
//			Setup();
	}

	async private void CloseConnection_Click(object sender, EventArgs e) {
		await Close();
	}

	private void Info_Click(object sender, EventArgs e) {
		if (ServerWebSocketsChannel != null) {
			Output.AppendLine($"State: {ServerWebSocketsChannel.State}");
			Output.AppendLine($"Role: {ServerWebSocketsChannel.LocalRole}");
		}
		if (ClientWebSocketsChannel != null) {
			Output.AppendLine($"State: {ClientWebSocketsChannel.State}");
			Output.AppendLine($"Role: {ClientWebSocketsChannel.LocalRole}");
		} else {
			Output.AppendLine($"WebSocketChannel is null");
		}
	}

	private void Reset_Click(object sender, EventArgs e) {
		if (DataSource != null) {

			return;
		}

		var localEndpoint = new IPEndPoint(IPAddress.Parse(MyIp.Text), (int)SendPort.Value);
		var remoteEndpoint = new IPEndPoint(IPAddress.Parse(SendIP.Text), (int)SendPort.Value);
		DataSource = new ServerWebSocketsDataSource<TestClass>(localEndpoint, remoteEndpoint, false, InitializeItem, UpdateItem, IdItem);

		Output.Text = String.Empty;
	}

	private void Clients_CellContentClick(object sender, DataGridViewCellEventArgs e) {

		if (e.ColumnIndex != 3) return;
		if (DataSource == null) return;

		var senderGrid = (DataGridView)sender;
		if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0) {
			var id = Clients.Rows[e.RowIndex].Cells[0].Value.ToString();
			if (id != null && id.Length > 1) {
				DataSource.CloseConnection(id);
			}
		}
	}
}
