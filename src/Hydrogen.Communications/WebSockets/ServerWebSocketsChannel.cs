using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.IO;

namespace Hydrogen.Communications {
	public class ServerWebSocketsChannel : ProtocolChannel, IDisposable {

		public event EventHandlerEx<WebSocketReceiveResult> ReceivedWebSocketMessage;
		IPEndPoint LocalEndpoint { get; }
		IPEndPoint RemoteEndpoint { get; }
		bool Secure { get; }
		TcpClient TcpClient { get; set; }
		TcpListener Server { get; set; }
		NetworkStream NetworkStream { get; set; }
		WebSocket WebSocket { get; set; }

		public ServerWebSocketsChannel(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint, bool secure) {
			LocalEndpoint = localEndpoint;
			RemoteEndpoint = remoteEndpoint;
			LocalRole = CommunicationRole.Server;
			Secure = secure;

			if (Secure) {
				throw new NotImplementedException("Secure Websockets is not available yet");
			}

			Server = new TcpListener(LocalEndpoint);
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

			var tcs = new TaskCompletionSourceEx();

			ReceivedWebSocketMessage += async message => {
				tcs.SetResult();
			};
			cancellationToken.Register(() => tcs.TrySetCanceled());

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

		protected override bool IsConnectionAlive() {
			throw new NotImplementedException();
			// HS 2022-01-30: disable due to porting issues .NET 5 -> .NET Standard 2.1
			//if (Server == null || Server.Server == null || //!Server.Pending() || !Server.Server.Connected ||
			//	TcpClient == null || !TcpClient.Connected || TcpClient.Client == null || !TcpClient.Client.Connected ||
			//	NetworkStream == null || NetworkStream.Socket == null || !NetworkStream.Socket.Connected ||
			//	WebSocket == null || WebSocket.State == WebSocketState.Aborted ||
			//							WebSocket.State == WebSocketState.Closed ||
			//							WebSocket.State == WebSocketState.CloseReceived ||
			//							WebSocket.State == WebSocketState.CloseSent ||
			//							WebSocket.State == WebSocketState.None) {
			//	return false;
			//}

			//return NetworkStream.Socket.Connected;
			////return Server.Server.Connected;
		}

		protected override async Task OpenInternal() {
			Server.Start();
			TcpClient = await Server.AcceptTcpClientAsync(); // will block here until a connection is made
			NetworkStream = TcpClient.GetStream();
			// "ws" probably does nothing and should be ignored
			WebSocket = WebSocket.CreateFromStream(NetworkStream, true, "ws", new TimeSpan(0, 30, 0)); // 30 Minute timeout, this seems to do nothing and is default at about 30 secs
			DoHandshake(NetworkStream);

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
			var buffer = new byte[1024];

			using (var memoryStream = new MemoryStream()) {

				while (true) {
					var received = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
					NotifyReceivedWebSocketMessage(received);
					memoryStream.Write(buffer, 0, received.Count);
					if (received.EndOfMessage) break;
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
SystemLog.Info("NotifyReceivedWebSocketMessage()");

			OnReceivedWebSocketMessage(result);
			ReceivedWebSocketMessage?.Invoke(result);
		}

		async static void DoHandshake(NetworkStream stream) {

SystemLog.Info("DoHandshake()");

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

SystemLog.Info("End DoHandshake()");
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
}
