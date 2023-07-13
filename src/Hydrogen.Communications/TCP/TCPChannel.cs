// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: David Price
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//TODO: needs rewrite, use stream extensions to build buffer

using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen.Communications;

public class TCPChannel : ProtocolChannel {

	public IPEndPoint LocalEndpoint { get; }
	public IPEndPoint RemoteEndpoint { get; }
	private TcpListener _tcpListener;

	public TCPChannel(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint, CommunicationRole role) {
		LocalEndpoint = localEndpoint;
		RemoteEndpoint = remoteEndpoint;
		LocalRole = role;
		_tcpListener = new TcpListener(LocalEndpoint.Address, LocalEndpoint.Port);
	}

	public override CommunicationRole LocalRole { get; }

	protected override async Task OpenInternal() {
		_tcpListener.Start();
		SystemLog.Info("TCPChannel Opened");
	}

	protected override async Task CloseInternal() {
		_tcpListener?.Stop();
		SystemLog.Info("TCPChannel Closed");
	}

	public override bool IsConnectionAlive() {
		return _tcpListener != null;
	}

	protected override async Task<byte[]> ReceiveBytesInternal(CancellationToken cancellationToken) {

//SystemLog.Info("TCPChannel About to Block Waiting for Connection");
		using (var client = await _tcpListener.AcceptTcpClientAsync().WithCancellationToken(cancellationToken)) {
//SystemLog.Info("TCPChannel Accepted Connection");
			using (var stream = client.GetStream()) {
				var buffer = new byte[1024];

				using (var memoryStream = new MemoryStream()) {
					while (stream.DataAvailable) {
						var size = await stream.ReadAsync(buffer, cancellationToken);
						memoryStream.Write(buffer, 0, size);
					}

					return memoryStream.ToArray();
				}
			}
		}
	}

	protected override async Task<bool> TrySendBytesInternal(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken) {

		try {
//SystemLog.Info($"TCP Channel Sending to: {RemoteEndpoint.Address} Port: {RemoteEndpoint.Port}");

			using (var sender = new TcpClient(RemoteEndpoint.Address.ToString(), RemoteEndpoint.Port)) {
				using (var stream = sender.GetStream()) {
					stream.WriteBytes(bytes.ToArray());
					return true;
				}
			}
		} catch (Exception e) {

			SystemLog.Info($"ERROR Sending {e.Message}");

			return false;
		}
	}
}
