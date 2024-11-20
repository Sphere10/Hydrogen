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

public class ServerWebSocketsChannel : ProtocolChannel, IDisposable {

	public event EventHandlerEx<WebSocketReceiveResult> ReceivedWebSocketMessage;
	IPEndPoint LocalEndpoint { get; }
	IPEndPoint RemoteEndpoint { get; }
	bool Secure { get; }
	TcpClient TcpClient { get; set; }
	TcpListener Server { get; set; }
	NetworkStream NetworkStream { get; set; }
	WebSocket WebSocket { get; set; }

	public ServerWebSocketsChannelHub Hub { get; set; }
	public bool CreatedHub { get; set; }
	public string InternalId { get; init; }

	public ServerWebSocketsChannel(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint, bool secure, bool hub = false) {
		if (Secure) {
			throw new NotImplementedException("Secure Websockets is not available yet");
		}

		InternalId = Guid.NewGuid().ToString();
		LocalEndpoint = localEndpoint;
		RemoteEndpoint = remoteEndpoint;
		LocalRole = CommunicationRole.Server;
		Secure = secure;
		Server = new TcpListener(LocalEndpoint);
		CloseInitiator = null;
		if (hub) {
			Hub = new ServerWebSocketsChannelHub(this);
			CreatedHub = true;
		}
	}

	public ServerWebSocketsChannel(ServerWebSocketsChannel channel, ServerWebSocketsChannelHub hub) {
		if (Secure) {
			throw new NotImplementedException("Secure Websockets is not available yet");
		}

		InternalId = Guid.NewGuid().ToString();
		LocalEndpoint = channel.LocalEndpoint;
		RemoteEndpoint = channel.RemoteEndpoint;
		LocalRole = CommunicationRole.Server;
		Secure = channel.Secure;
		WebSocket = channel.WebSocket;

		if (Secure) {
			throw new NotImplementedException("Secure Websockets is not available yet");
		}

		Hub = hub;
		Server = channel.Server;
		CloseInitiator = null;
	}

	public override CommunicationRole LocalRole { get; }

	public CommunicationRole? CloseInitiator { get; set; }

	public override Task Close() {
		CloseInitiator ??= this.LocalRole;
		return base.Close();
	}

	async Task RespondToCloseMessage() {
		if (WebSocket.State != WebSocketState.Closed) {
			await WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
		}
	}

	protected override async Task BeginClose(CancellationToken cancellationToken) {
		if (CloseInitiator != LocalRole)
			return;

		var tcs = new TaskCompletionSource();

		ReceivedWebSocketMessage += async message => { tcs.SetResult(); };
		//			cancellationToken.Register(() => tcs.TrySetCanceled());

		await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

		await tcs.Task;
	}

	protected override async Task CloseInternal() {
		TcpClient.Close();
		TcpClient.Dispose();
		TcpClient = null;

		Server.Server.Close();
		Server.Stop();
		Server = null;

		NetworkStream.Close();
		NetworkStream.Flush();
		NetworkStream.Dispose();
		NetworkStream = null;

		WebSocket = null;
	}

	public override bool IsConnectionAlive() {
		if (WebSocket == null) return false;
		return WebSocket.State == WebSocketState.Open;
	}

	protected override async Task OpenInternal() {
		if (!Server.Server.IsBound) {
			Server.Start();
			SystemLog.Info("Server Started");
		}

		TcpClient = await Server.AcceptTcpClientAsync(); // will block here until a connection is made
		SystemLog.Info("Server New Connection");

		NetworkStream = TcpClient.GetStream();
		// "ws" probably does nothing and should be ignored
		WebSocket = WebSocket.CreateFromStream(NetworkStream, true, "ws", new TimeSpan(0, 30, 0)); // 30 Minute timeout, this seems to do nothing and is default at about 30 secs
		DoHandshake(NetworkStream);

		// send back the Id for this channel to the client
		// if this could be done in the handshake, that would be even better
		await WebSocket.SendAsync(Encoding.ASCII.GetBytes(InternalId), WebSocketMessageType.Text, true, CancellationToken.None);

		//Handle the response to close
		ReceivedWebSocketMessage += async msg => {

			SystemLog.Info($"XXXX Handle the response to close ONLY");
			SystemLog.Info($"ServerWebSocketsChannel Recieved Data Created Hub {CreatedHub}");

			if (msg.MessageType == WebSocketMessageType.Close && CloseInitiator == null) {
				CloseInitiator = LocalRole switch {
					CommunicationRole.Server => CommunicationRole.Client,
					CommunicationRole.Client => CommunicationRole.Server,
				};
				// handle close response
				await RespondToCloseMessage();
				CloseInitiator ??= this.LocalRole;
				await CloseInternal();
			}

			//if (!CreatedHub) {
			//	Hub.ConnectionMade
			//	ServerWebSocketsDataSource
			//	ReceivedWebSocketMessage.Invoke(msg);
			//}
		};

		if (Hub != null) Hub.ConnectionMade(this);
	}

	protected override async Task<byte[]> ReceiveBytesInternal(CancellationToken cancellationToken) {
		var buffer = new byte[1024];
		using (var memoryStream = new MemoryStream()) {
			while (true) {
				var received = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
				NotifyReceivedWebSocketMessage(received);
				memoryStream.Write(buffer, 0, received.Count);
				if (received.EndOfMessage) break;
			}

			// if this is a hub, need to pass into to it
			if (Hub != null) {
				if (memoryStream.Length > 0) {
					Hub.ReceivedBytesExternal(memoryStream.ToArray());
				}
			}

			return memoryStream.ToArray();
		}
	}

	protected override async Task<bool> TrySendBytesInternal(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken) {
		await WebSocket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);

		return true;
	}

	protected virtual void OnReceivedWebSocketMessage(WebSocketReceiveResult result) {
	}

	private void NotifyReceivedWebSocketMessage(WebSocketReceiveResult result) {
		OnReceivedWebSocketMessage(result);
		ReceivedWebSocketMessage?.Invoke(result);
	}

	async static void DoHandshake(NetworkStream stream) {
		using (var memoryStream = new MemoryStream()) {
			var size = 0;
			var buffer = new byte[1024];

			while ((size = await stream.ReadAsync(buffer, CancellationToken.None)) > 0) {
				memoryStream.Write(buffer, 0, size);
				if (!stream.DataAvailable) break; // this might be improvable
			}

			var array = memoryStream.ToArray();

			if (array.Length > 0) {
				SwitchHttpToWebSockets(array, stream);
			}
		}

		//SystemLog.Info("DoHandshake() Completed");
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

			                                                                            // Try and send back the ID now if possible
			                                                                            // will stop any simultaneous stuff
			                                                                            //+ "Sec-WebSocket-Protocol: " + Guid.NewGuid() + eol
			                                                                            //+ "Sec-WebSocket-Protocol: " + "ocpp1.2" + eol
			                                                                            //+ "Sec-WebSocket-Key: " + "ZZZ" + eol
			                                                                            + eol);
			stream.Write(response, 0, response.Length);
		}
	}
}
