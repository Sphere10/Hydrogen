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

		private EndpointMessage ReadInternal(TcpClient stream) {
			int bytesRead = 0;
			byte[] messageBytes = new byte[MaxMessageSize];
			bytesRead = stream.GetStream().Read(messageBytes, 0, MaxMessageSize);
			return new EndpointMessage(messageBytes, new TcpEndPoint(stream));
		}
		//Readmessage create a message with client stream so WriteMessage can reply back to it
		public EndpointMessage ReadMessage() {
			TcpClient client = tcpListener.AcceptTcpClient();
			return ReadInternal(client);
		}

		//Write a message to a specific client (not to the Listening socket).
		public void WriteMessage(EndpointMessage message) {
			Debug.Assert(message.streamContext != null);
			message.streamContext.WriteMessage(message);
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