using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {
	public class UDPChannel : ProtocolChannel, IDisposable {

		private IPEndPoint _remoteEndpoint;
		private UdpClient _client;

		public UDPChannel(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint, CommunicationRole role) {
			_client = new UdpClient(localEndpoint);
			_remoteEndpoint = remoteEndpoint;
			LocalRole = role;
		}

		public override CommunicationRole LocalRole { get; }

		protected override async Task OpenInternal() {
			// UDP has no concept of opening the connection
			_client.Connect(_remoteEndpoint);
		}

		protected override async Task CloseInternal() {
			// UDP has no concept of closing the connection
			_client.Close();
		}

		protected override async Task<byte[]> ReceiveBytesInternal(CancellationToken cancellationToken) {
			var result = await _client.ReceiveAsync().WithCancellationToken(cancellationToken);
			return result.Buffer;
		}

		protected override async Task<bool> TrySendBytesInternal(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken) {
			try {
				var sentLength = await _client.SendAsync(bytes.ToArray(), bytes.Length).WithCancellationToken(cancellationToken);
				if (sentLength != bytes.Length)
					return false;
			} catch (Exception error) {
				return false;
			}
			return true;
		}

		protected override bool IsConnectionAlive() {
			// UDP has no concept of a connection being alive
			return true;
		}

		public new void Dispose() {
			_client?.Dispose();
		}
	}
}
