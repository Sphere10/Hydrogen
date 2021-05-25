using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace Sphere10.Framework.Communications.RPC {
	//Implement a TCP communication endpoint listnener
	public class TcpEndPointListener : IEndPoint {
		private TcpListener		tcpListener;
		private int				maxConnections = 5;
		public int				MaxMessageSize { get; set; } = 4096;
		public TcpEndPointListener(bool isLocal, int port, int maxConnection) {
			tcpListener = new TcpListener(isLocal ? IPAddress.Loopback : IPAddress.Any, port);
			maxConnections = maxConnection;
		}

		public string GetDescription() { 
			return tcpListener != null ? $"{((IPEndPoint)tcpListener.LocalEndpoint).Address.ToString()}:{((IPEndPoint)tcpListener.LocalEndpoint).Port}" : "";
		}

		//wait for connection and return client's endpoint
		public IEndPoint WaitForMessage() {
			TcpEndPoint newClient = null;
			try {
				TcpClient client = tcpListener.AcceptTcpClient();
				newClient = new TcpEndPoint(client);
			}
			catch(Exception e) {
				throw;
			}

			return newClient;
		}

		//Implement single-thread blocking Listen+Read of an EndPointMessage
		public EndpointMessage ReadMessage() {
			IEndPoint client = WaitForMessage();
			return client.ReadMessage();
		}

		//Write a message to a specific client (not to the Listening socket).
		public void WriteMessage(EndpointMessage message) {
			Debug.Assert(message.stream != null);
			message.stream.WriteMessage(message);
		}

		public bool IsOpened() {
			return tcpListener.Server.IsBound;
		}

		public void Start() {
			tcpListener.Start(maxConnections);
		}

		public void Stop() {
			tcpListener.Stop();
		}
	}
}