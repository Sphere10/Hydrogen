using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace Sphere10.Framework.Communications.RPC {
	//Implement a TCP communication endpoint
	public class TcpEndPoint : IEndPoint {
		protected TcpClient TcpSocket;
		protected int Port;
		protected string Address;
		public int MaxMessageSize { get; set; } = 4096;

		public TcpEndPoint(string remoteAddress, int remotePort) {
			Port = remotePort;
			Address = remoteAddress;
		}

		public TcpEndPoint(TcpClient clientSocket) {
			TcpSocket = clientSocket;
		}

		public ulong GetUID() {
			return TcpSocket != null ? (ulong)TcpSocket.Client.Handle.ToInt64() : 0;
		}

		public string GetDescription() {
			return TcpSocket != null ? $"{((IPEndPoint)TcpSocket.Client.RemoteEndPoint).Address.ToString()}:{((IPEndPoint)TcpSocket.Client.RemoteEndPoint).Port}" : "";
		}

		public IEndPoint WaitForMessage() {
			return this;
		}

		public virtual EndpointMessage ReadMessage() {
			if (!IsOpened())
				Start();

			int bytesRead = 0;
			byte[] messageBytes = new byte[MaxMessageSize];
			bytesRead = TcpSocket.GetStream().Read(messageBytes, 0, MaxMessageSize);
			TcpSecurityPolicies.ValidateJsonQuality(messageBytes, bytesRead);
			Array.Resize<byte>(ref messageBytes, bytesRead);
			return new EndpointMessage(messageBytes, this);
		}

		public virtual void WriteMessage(EndpointMessage message) {
			if (!IsOpened())
				Start();

			message.Stream = this;
			TcpSocket.GetStream().Write(message.MessageData, 0, message.MessageData.Length);
		}

		public virtual bool IsOpened() {
			return TcpSocket != null && TcpSocket.Client != null && (TcpSocket.Client.Poll(0, SelectMode.SelectWrite) && !TcpSocket.Client.Poll(0, SelectMode.SelectError));
		}

		public virtual void Start() {
			if (Address != null) {
				TcpSocket = new TcpClient();
				TcpSocket.Connect(Address, Port);
			}
		}

		public virtual void Stop() {
			TcpSocket?.Close();
			TcpSocket = null;
		}
	}
}