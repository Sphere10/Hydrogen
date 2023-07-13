// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: David Price
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.IO;

namespace Hydrogen.Communications;

public class WebSocketsChannel : ProtocolChannel, IDisposable {

	public IPEndPoint LocalEndpoint { get; }
	public IPEndPoint RemoteEndpoint { get; }
	public CommunicationRole Role { get; }
	bool Secure { get; }
	TcpClient TcpClient { get; set; }

	TcpListener Server { get; set; }
	NetworkStream NetWorkStream { get; set; }
	WebSocket WebSocket { get; set; }

	public WebSocketsChannel(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint, CommunicationRole role, bool secure) {
		LocalEndpoint = localEndpoint;
		RemoteEndpoint = remoteEndpoint;
		Role = role;
		Secure = secure;

		if (Secure) {
			throw new NotImplementedException("Secure Websockets is not available yet");
		}

		if (role == CommunicationRole.Server) {
			Server = new TcpListener(localEndpoint);
		} else {

		}
	}

	public override CommunicationRole LocalRole => throw new NotImplementedException();

	protected override async Task CloseInternal() {
		if (Role == CommunicationRole.Server) {
			Server?.Stop();
		} else {

		}
	}

	public override bool IsConnectionAlive() {
		throw new NotImplementedException();
		// HS 2022-01-30: disable due to porting issues .NET 5 -> .NET Standard 2.1
		//if (Role == CommunicationRole.Server) {

		//	return NetWorkStream.Socket.Connected;

		//	//return Server.Server.Connected;
		//} else {
		//	// finish code
		//	return true;
		//}
	}

	protected override async Task OpenInternal() {

		if (Role == CommunicationRole.Server) {
			Server.Start();
			TcpClient = await Server.AcceptTcpClientAsync();
			NetWorkStream = TcpClient.GetStream();
			// "ws" probably does nothing and should be ignored
			WebSocket = WebSocket.CreateFromStream(NetWorkStream, true, "ws", new TimeSpan(0, 30, 0)); // 30 Minute timeout

			DoHandshake(NetWorkStream);

			SystemLog.Info("WebSocketChannel HTTP to WebSockets Handshake Completed");
		} else {
			throw new NotImplementedException();
		}
	}

	protected override async Task<byte[]> ReceiveBytesInternal(CancellationToken cancellationToken) {

		if (Role == CommunicationRole.Server) {

			Byte[] buffer = new byte[1024];

			using (var memoryStream = new MemoryStream()) {

				while (true) {

					var received = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
					memoryStream.Write(buffer, 0, received.Count);

					if (received.EndOfMessage) break;
				}

				SystemLog.Info("WebSocketChannel ReceiveBytesInternal");

				return memoryStream.ToArray();
			}
		} else {
			throw new NotImplementedException();
		}
	}

	protected override async Task<bool> TrySendBytesInternal(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken) {
		if (Role == CommunicationRole.Server) {
			await WebSocket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);

			return true;
		} else {
			throw new NotImplementedException();
		}
	}


	static void DoHandshake(NetworkStream stream) {

		using (var memoryStream = new MemoryStream()) {
			var size = 0;
			var buffer = new byte[1024];
			while (stream.DataAvailable && (size = stream.Read(buffer)) > 0) {
				memoryStream.Write(buffer, 0, size);
			}

			var array = memoryStream.ToArray();
			if (array.Length > 0) {
				SwitchHttpToWebSockets(array, stream);
			}
		}
	}

	static void SwitchHttpToWebSockets(byte[] bytes, Stream stream) {
		var text = Encoding.UTF8.GetString(bytes);

		// this is the initial connection from a client
		// need to do the handshake
		if (new System.Text.RegularExpressions.Regex("^GET").IsMatch(text)) {
			const string eol = "\r\n"; // HTTP/1.1 defines the sequence CR LF as the end-of-line marker

			Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + eol
			                                                                            + "Connection: Upgrade" + eol
			                                                                            + "Upgrade: websocket" + eol
			                                                                            + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
				                                                                            System.Security.Cryptography.SHA1.Create().ComputeHash(
					                                                                            Encoding.UTF8.GetBytes(
						                                                                            new System.Text.RegularExpressions.Regex("Sec-WebSocket-Key: (.*)").Match(text).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
					                                                                            )
				                                                                            )
			                                                                            ) + eol
			                                                                            + eol);
			stream.Write(response, 0, response.Length);
		}
	}
}
