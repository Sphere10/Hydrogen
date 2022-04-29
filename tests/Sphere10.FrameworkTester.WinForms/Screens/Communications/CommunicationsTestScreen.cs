//-----------------------------------------------------------------------
// <copyright file="ScreenA.cs" company="Sphere 10 Software">
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

using System;
using System.Text;
using System.Windows.Forms;
using Sphere10.Framework.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Sphere10.Framework.Communications;
using Sphere10.Framework;
using System.Threading.Tasks;

namespace Sphere10.FrameworkTester.WinForms
{
	public partial class CommunicationsTestScreen : ApplicationScreen
	{
		const int NodeDiscoveryPort = 21000;
		CommunicationRole CommunicationRole { get; set; }

		UDPChannel UDPChannel { get; set; }
		TCPChannel TCPChannel { get; set; }
		//WebSocketsChannel  WebSocketsChannel { get; set; }
		ServerWebSocketsChannel ServerWebSocketsChannel { get; set; }
		ClientWebSocketsChannel ClientWebSocketsChannel { get; set; }

		private ServerWebSocketsDataSource<TestClass> DataSource { get; set; }

		public CommunicationsTestScreen() {
            InitializeComponent();
			SystemLog.RegisterLogger(new TextBoxLogger(Output));
			SystemLog.RegisterLogger(new FileAppendLogger("c:/temp/test.txt", true));
        }

		private void WebSocketsScreen_Load(object sender, EventArgs e)
		{
			SendPort.Value = 80;
			var localHost = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ipAddress in localHost.AddressList)
			{
				if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6) continue;

				var ipString = ipAddress.ToString();
				var addressFamiliy = ipAddress.AddressFamily;
									
				MyIPs.Items.Add(ipString).SubItems.Add(addressFamiliy.ToString());

				if (ipString.Contains("192.168"))
				{
					MyIp.Text = ipString;
				}
			}

			if (MyIp.Text == "192.168.1.108") // virtual computer
			{
				SendIP.Text = "192.168.1.171";// "192.168.1.106";
				Message.Text = "Message from TWO";
				CommunicationRole = CommunicationRole.Client;
				//CommunicationRole = CommunicationRole.Server;
			}
			else // Actual Desktop
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
			DataSource = new ServerWebSocketsDataSource<TestClass>(localEndpoint, remoteEndpoint, false);
		}

		private void Send_Click(object sender, EventArgs e)
		{
			var data = Encoding.ASCII.GetBytes(Message.Text);

			if (Protocol.Text == "UDP" && UDPChannel != null)
			{
				UDPChannel.TrySendBytes(data);
			}
			else if (Protocol.Text == "TCP" && TCPChannel != null)
			{
				TCPChannel.TrySendBytes(data);
			}
			else if (Protocol.Text == "WebSockets")
			{
				if (CommunicationRole == CommunicationRole.Server)
				{
					ServerWebSocketsChannel?.TrySendBytes(data);
				}
				else
				{
					ClientWebSocketsChannel?.TrySendBytes(data);
				}
			}
		}

		async Task Close()
		{
			if (UDPChannel != null) {
				await UDPChannel.Close();
				UDPChannel = null;
			}

			if (TCPChannel != null) {
				await TCPChannel.Close();
				TCPChannel = null;
			}

			if (CommunicationRole == CommunicationRole.Server && ServerWebSocketsChannel != null)
			{
				await ServerWebSocketsChannel.Close();
				ServerWebSocketsChannel = null;
			}
			else if (CommunicationRole == CommunicationRole.Client && ClientWebSocketsChannel != null)
			{
				await ClientWebSocketsChannel.Close();
				ClientWebSocketsChannel = null;
			}
		}

		async void Setup()
		{
			var localEndpoint = new IPEndPoint(IPAddress.Parse(MyIp.Text), (int)SendPort.Value);
			var remoteEndpoint = new IPEndPoint(IPAddress.Parse(SendIP.Text), (int)SendPort.Value);

			if (Protocol.Text == "UDP")
			{
				UDPChannel = new UDPChannel(localEndpoint, remoteEndpoint, CommunicationRole.Client);
				UDPChannel.ReceivedBytes += delegate (ReadOnlyMemory<byte> memory)
				{
					Output.Invoke((MethodInvoker)delegate
					{
						Output.AppendLine("UDP: " + Encoding.ASCII.GetString(memory.Span));
					});
				};
				await UDPChannel.Open();
			}
			else if (Protocol.Text == "TCP")
			{
				TCPChannel = new TCPChannel(localEndpoint, remoteEndpoint, CommunicationRole.Client);
				TCPChannel.ReceivedBytes += delegate (ReadOnlyMemory<byte> memory)
				{
					Output.Invoke((MethodInvoker)delegate
					{
						Output.AppendLine("TCP: " + Encoding.ASCII.GetString(memory.Span));
					});
				};
				await TCPChannel.Open();
			}
			else if (Protocol.Text == "WebSockets")
			{
				var secure = false;
				var socketType = secure ? "wss" : "ws";
				if (CommunicationRole == CommunicationRole.Server)
				{
					ServerWebSocketsChannel = new ServerWebSocketsChannel(localEndpoint, remoteEndpoint, secure);
					ServerWebSocketsChannel.ReceivedBytes += delegate (ReadOnlyMemory<byte> memory)
					{
						Output.Invoke((MethodInvoker)delegate
						{
							Output.AppendLine("WebSockets: " + Encoding.ASCII.GetString(memory.Span));
						});
					};
					await ServerWebSocketsChannel.Open();
				}
				else // client
				{
					ClientWebSocketsChannel = new ClientWebSocketsChannel($"{socketType}://{remoteEndpoint.Address}:{remoteEndpoint.Port}", secure);
					ClientWebSocketsChannel.ReceivedBytes += delegate (ReadOnlyMemory<byte> memory)
					{
						Output.Invoke((MethodInvoker)delegate
						{
							Output.AppendLine("WebSockets: " + Encoding.ASCII.GetString(memory.Span));
						});
					};
					await ClientWebSocketsChannel.Open();
				}
			}
		}

		async private void Protocol_SelectedValueChanged(object sender, EventArgs e)
		{
			Output.AppendLine("---------------------------------------------");
			await Close();
//			Setup();
		}

		async private void CloseConnection_Click(object sender, EventArgs e)
		{
			await Close();
		}

		private void Info_Click(object sender, EventArgs e)
		{
			if (ServerWebSocketsChannel != null)
			{
				Output.AppendLine($"State: {ServerWebSocketsChannel.State}");
				Output.AppendLine($"Role: {ServerWebSocketsChannel.LocalRole}");
			}
			if (ClientWebSocketsChannel != null)
			{
				Output.AppendLine($"State: {ClientWebSocketsChannel.State}");
				Output.AppendLine($"Role: {ClientWebSocketsChannel.LocalRole}");
			}
			else
			{
				Output.AppendLine($"WebSocketChannel is null");
			}
		}

		private void Reset_Click(object sender, EventArgs e)
		{

		}
	}
}

