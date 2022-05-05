using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace Hydrogen.Communications.RPC {
	//Implement a TCP communication endpoint listnener
	public class TcpEndPointListener : IEndPoint {
		private ulong _uid = (ulong)Tools.Maths.RNG.Next();
		private TcpListener		TcpListener;
		private int				MaxListeners = 5;
		public int				MaxMessageSize { get; set; } = 4096;

		public TcpEndPointListener(bool isLocal, int port, int maxListener) {
			TcpListener = new TcpListener(isLocal ? IPAddress.Loopback : IPAddress.Any, port);
			MaxListeners = maxListener;
		}

		public string GetDescription() { 
			return TcpListener != null ? $"{((IPEndPoint)TcpListener.LocalEndpoint).Address.ToString()}:{((IPEndPoint)TcpListener.LocalEndpoint).Port}" : "";
		}
		public ulong GetUID() { return _uid; }

		//wait for connection and return client's endpoint
		public IEndPoint WaitForMessage() {
			TcpEndPoint newClient = null;
			TcpClient client = TcpListener.AcceptTcpClient();
			newClient = new TcpEndPoint(client);
			return newClient;
		}

		//Implement single-thread blocking Listen+Read of an EndPointMessage
		public EndpointMessage ReadMessage() {
			IEndPoint client = WaitForMessage();
			return client.ReadMessage();
		}

		//Write a message to a specific client (not to the Listening socket).
		public void WriteMessage(EndpointMessage message) {
			Debug.Assert(message.Stream != null);
			message.Stream.WriteMessage(message);
		}

		public bool IsOpened() {
			return TcpListener.Server.IsBound;
		}

		public void Start() {
			TcpListener.Start(MaxListeners);
		}

		public void Stop() {
			TcpListener.Stop();
		}
	}
}