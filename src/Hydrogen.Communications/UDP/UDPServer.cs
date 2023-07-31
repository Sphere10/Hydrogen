// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Hydrogen.Communications;

public class UDPServer : AsyncDisposable {

	public EventHandlerEx<UDPServerChannel> ChannelCreated;
	public EventHandlerEx<UDPServerChannel> ChannelDestroyed;

	private readonly SynchronizedDictionary<IPEndPoint, UDPServerChannel> _channels;
	private CancellationTokenSource _stopServerTrigger;

	public UDPServer(int port) {
		_channels = new SynchronizedDictionary<IPEndPoint, UDPServerChannel>();
		_stopServerTrigger = new CancellationTokenSource();
	}

	public IEnumerable<UDPServerChannel> Channels => _channels.Values;

	public int Port { get; }

	public bool Running { get; private set; }

	public async Task Run() {
		Guard.Ensure(!Running, "Already running");
		var client = new UdpClient(new IPEndPoint(IPAddress.Any, Port));
		while (true) {
			// TODO: refactor this loop with a thread. The task async/await overhead for a single packet is disgustingly unacceptable
			// and evidence that the whole async/await paradigm is broken in .NET! :(
			var result = await client.ReceiveAsync().WithCancellationToken(_stopServerTrigger.Token).IgnoringCancellationException();
			if (_stopServerTrigger.IsCancellationRequested)
				break;
			var endpointAccepted = IsEndpointAccepted(result.RemoteEndPoint);
			using (_channels.EnterWriteScope()) {
				var hasChannel = _channels.TryGetValue(result.RemoteEndPoint, out var channel);
				switch (hasChannel) {
					case false when endpointAccepted:
						// Accepted new channel, add it
						channel = await CreateChannel(result.RemoteEndPoint);
						await channel.Open();
						channel.InjectIncomingBytes(result.Buffer);
						break;
					case false when !endpointAccepted:
						// Unsolicited UDP traffic is ignored hereb
						break;
					case true when endpointAccepted:
						// Traffic for existing channel
						await channel.Open();
						channel.InjectIncomingBytes(result.Buffer);
						break;
					case true when !endpointAccepted:
						// Not accepted anymore, remove channel
						await DisposeChannel(channel);
						break;
				}
			}
		}

		// close up all the channels
		await _channels.Values.ForEachAsync(c => DisposeChannel(c).WithExceptionHandler(error => SystemLog.Exception(error)));
		_channels.Clear();
	}


	public void Stop() {
		Guard.Ensure(Running, "Not running");
		_stopServerTrigger.Cancel();
		_stopServerTrigger = new CancellationTokenSource();
	}

	protected virtual bool IsEndpointAccepted(IPEndPoint channel) => true;


	protected override async ValueTask FreeManagedResourcesAsync() {
		if (Running)
			Stop();
	}

	private async Task<UDPServerChannel> CreateChannel(IPEndPoint endpoint) {
		var channel = new UDPServerChannel(endpoint, this);
		await channel.Close();
		_channels.Add(endpoint, channel);
		ChannelDestroyed?.InvokeAsync(channel);
		return channel;
	}

	private async Task DisposeChannel(UDPServerChannel channel) {
		await channel.Close();
		_channels.Remove(channel.Endpoint);
		ChannelDestroyed?.InvokeAsync(channel);
	}


	public class UDPServerChannel : ProtocolChannel {
		private readonly UdpClient _client;
		public UDPServerChannel(IPEndPoint endpoint, UDPServer parent) {
			Endpoint = endpoint;
			_client = new UdpClient(endpoint);
		}

		public IPEndPoint Endpoint { get; }

		internal void InjectIncomingBytes(byte[] bytes) {
			if (bytes?.Length > 0)
				base.NotifyReceivedBytes(bytes);
		}

		public override CommunicationRole LocalRole { get; }

		protected override async Task OpenInternal() {
			// "connection state" managed by parent UDPServer
		}

		protected override async Task CloseInternal() {
			// "connection state" managed by parent UDPServer
		}

		protected override void StartReceiveLoop() {
			// the receive loop is managed by the parent UDPServer
		}

		protected override async Task StopReceiveLoop() {
			// the receive loop is managed by the parent UDPServer
		}

		protected override async Task<byte[]> ReceiveBytesInternal(CancellationToken cancellationToken) {
			// bytes are injected into this channel by parent UDP Server and this never gets called
			throw new NotSupportedException();
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

		public override bool IsConnectionAlive() {
			// UDP has no concept of a connection being alive
			return true;

		}
	}
}
