using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace Sphere10.Framework.Communications.RPC {
	//Implement a TCP communication endpoint
	public class TcpEndPoint : IEndPoint {
		protected TcpClient tcpSocket;
		protected int port;
		protected string address;
		public int MaxMessageSize { get; set; } = 4096;

		public TcpEndPoint(string remoteAddress, int remotePort) {
			port = remotePort;
			address = remoteAddress;
		}

		public TcpEndPoint(TcpClient clientSocket) {
			tcpSocket = clientSocket;
		}

		public ulong GetUID() {
			return tcpSocket != null ? (ulong)tcpSocket.Client.Handle.ToInt64() : 0;
		}

		public string GetDescription() {
			return tcpSocket != null ? $"{((IPEndPoint)tcpSocket.Client.RemoteEndPoint).Address.ToString()}:{((IPEndPoint)tcpSocket.Client.RemoteEndPoint).Port}" : "";
		}

		public IEndPoint WaitForMessage() {
			return this;
		}

		public virtual EndpointMessage ReadMessage() {
			if (!IsOpened())
				Start();

			int bytesRead = 0;
			byte[] messageBytes = new byte[MaxMessageSize];
			bytesRead = tcpSocket.GetStream().Read(messageBytes, 0, MaxMessageSize);
			TcpSecurityPolicies.ValidateJsonQuality(messageBytes, bytesRead);
			Array.Resize<byte>(ref messageBytes, bytesRead);
			return new EndpointMessage(messageBytes, this);
		}

		public virtual void WriteMessage(EndpointMessage message) {
			if (!IsOpened())
				Start();

			message.stream = this;
			tcpSocket.GetStream().Write(message.messageData, 0, message.messageData.Length);
		}

		public virtual bool IsOpened() {
			return tcpSocket != null && tcpSocket.Client != null && (tcpSocket.Client.Poll(0, SelectMode.SelectWrite) && !tcpSocket.Client.Poll(0, SelectMode.SelectError));
		}

		public virtual void Start() {
			if (address != null) {
				tcpSocket = new TcpClient();
				tcpSocket.Connect(address, port);
			}
		}

		public virtual void Stop() {
			tcpSocket?.Close();
			tcpSocket = null;
		}
	}
}