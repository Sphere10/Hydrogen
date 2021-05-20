using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace Sphere10.Framework.Communications.RPC {
	//Implement a TCP communication endpoint
	public class TcpEndPoint : IEndPoint {
		protected TcpClient tcpSocket;
		protected int		port;
		protected string	address;
		public int			MaxMessageSize { get; set; } = 4096;
		public TcpEndPoint(string remoteAddress, int remotePort) {
			port = remotePort;
			address = remoteAddress;
		}
		public TcpEndPoint(TcpClient clientSocket) {
			tcpSocket = clientSocket;
		}

		public virtual EndpointMessage ReadMessage() {
			if (!IsOpened())
				Start();

			int bytesRead = 0;
			byte[] messageBytes = new byte[MaxMessageSize];
			bytesRead = tcpSocket.GetStream().Read(messageBytes, 0, MaxMessageSize);
			return new EndpointMessage(messageBytes, this);
		}

		public virtual void WriteMessage(EndpointMessage message) {
			if (!IsOpened())
				Start();

			message.streamContext = this;
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
		}
	}
}