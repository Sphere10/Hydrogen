// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: David Price
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.IO;
using System.Text;

namespace Hydrogen.Communications;

public class ClientWebSocketsChannel : ProtocolChannel, IDisposable {

	public event EventHandlerEx<WebSocketReceiveResult> ReceivedWebSocketMessage;
	string URI { get; }
	bool Secure { get; }
	ClientWebSocket ClientWebSocket { get; set; }

	public string Id { get; set; }

	public ClientWebSocketsChannel(string uri, bool secure) {
		URI = uri;
		LocalRole = CommunicationRole.Client;
		Secure = secure;

		if (Secure) {
			throw new NotImplementedException("Secure Websockets is not available yet");
		}

		ClientWebSocket = new ClientWebSocket();
	}

	public override CommunicationRole LocalRole { get; }

	public CommunicationRole? CloseInitiator { get; set; }

	public override Task Close() {
		CloseInitiator ??= this.LocalRole;
		return base.Close();
	}

	async Task RespondToCloseMessage() {
		if (ClientWebSocket.State != WebSocketState.Closed) {
			await ClientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
		}
	}

	protected override async Task BeginClose(CancellationToken cancellationToken) {
		if (CloseInitiator != LocalRole)
			return;

		var tcs = new TaskCompletionSource();

		ReceivedWebSocketMessage += async message => { tcs.SetResult(); };
		cancellationToken.Register(() => tcs.TrySetCanceled());

		await ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
		await tcs.Task;
	}

	protected override async Task CloseInternal() {
		ClientWebSocket = null;
	}

	public override bool IsConnectionAlive() {
		return ClientWebSocket.State == WebSocketState.Open;
	}

	protected override async Task OpenInternal() {

		try {
			await ClientWebSocket.ConnectAsync(new Uri(URI), CancellationToken.None);

			using (var memoryStream = new MemoryStream()) {
				var buffer = new byte[1024];
				while (true) {
					var received = await ClientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
					memoryStream.Write(buffer, 0, received.Count);
					if (received.EndOfMessage) break;
				}

				Id = Encoding.ASCII.GetString(memoryStream.ToArray());
			}

			SystemLog.Info($"ID: {Id}");
		} catch (Exception ex) {

		}

		// Handle the response to close
		ReceivedWebSocketMessage += async msg => {
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
		};
	}

	protected override async Task<byte[]> ReceiveBytesInternal(CancellationToken cancellationToken) {

		//SystemLog.Info($"ReceiveBytesInternal()");

		var buffer = new byte[1024];

		using (var memoryStream = new MemoryStream()) {

			while (true) {
				var received = await ClientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
				NotifyReceivedWebSocketMessage(received);
				memoryStream.Write(buffer, 0, received.Count);
				if (received.EndOfMessage) break;
			}

			return memoryStream.ToArray();
		}
	}

	protected override async Task<bool> TrySendBytesInternal(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken) {
		await ClientWebSocket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);

		return true;
	}

	protected virtual void OnReceivedWebSocketMessage(WebSocketReceiveResult result) {
	}

	private void NotifyReceivedWebSocketMessage(WebSocketReceiveResult result) {
		OnReceivedWebSocketMessage(result);
		ReceivedWebSocketMessage?.Invoke(result);
	}
}
