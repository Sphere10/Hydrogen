// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Net.Sockets;
using System.Net;

namespace Hydrogen.Communications.RPC;

//Implement a TCP communication endpoint
public class TcpEndPoint : IEndPoint {
	private ulong _uid = (ulong)Tools.Maths.RNG.Next();
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
		return _uid;
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
		if (message.MessageData.Length > 0)
			TcpSocket.GetStream().Write(message.MessageData, 0, message.MessageData.Length);
	}

	public virtual bool IsOpened() {
		if (TcpSocket == null || TcpSocket.Client == null)
			return false;

		bool part1 = TcpSocket.Client.Poll(1000, SelectMode.SelectRead);
		bool part2 = (TcpSocket.Client.Available == 0);
		bool isConnected = !(part1 && part2);
		bool isOpen = (TcpSocket.Client.Poll(0, SelectMode.SelectWrite) && !TcpSocket.Client.Poll(0, SelectMode.SelectError));
		return isOpen && isConnected;
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
