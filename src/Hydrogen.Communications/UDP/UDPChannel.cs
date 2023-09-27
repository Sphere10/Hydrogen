// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen.Communications;

public class UDPChannel : ProtocolChannel, IDisposable {
	private readonly UdpClient _client;

	public UDPChannel(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint, CommunicationRole role) {

		if (localEndpoint.Address == IPAddress.Broadcast) {

			var localHost = Dns.GetHostEntry(Dns.GetHostName());

			var myIPAddress = string.Empty;
			foreach (var ipAddress in localHost.AddressList) {
				if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6) continue;

				var ipString = ipAddress.ToString();
				var addressFamiliy = ipAddress.AddressFamily;

				if (ipString.Contains("192.168")) {
					myIPAddress = ipString;
				}
			}

			_client = new UdpClient();
//				_client.Client.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), localEndpoint.Port));
			_client.Client.Bind(new IPEndPoint(IPAddress.Parse(myIPAddress), localEndpoint.Port));
		} else {
			_client = new UdpClient(localEndpoint);
		}

		//		if (remoteEndpoint.Address == IPAddress.Broadcast) {
		//			remoteEndpoint.Address = IPAddress.Any;
		//		}

		LocalEndpoint = localEndpoint;
		RemoteEndpoint = remoteEndpoint;
		LocalRole = role;
	}

	public override CommunicationRole LocalRole { get; }

	public IPEndPoint LocalEndpoint { get; }

	public IPEndPoint RemoteEndpoint { get; }

	protected override async Task OpenInternal() {
		_client?.Connect(RemoteEndpoint);
		SystemLog.Info($"UDPChannel Opened Listening On {LocalEndpoint.Address}:{LocalEndpoint.Port}  to  {RemoteEndpoint.Address}:{RemoteEndpoint.Port}");
	}

	protected override async Task CloseInternal() {
		_client?.Close();
		SystemLog.Info("UDPChannel Closed");
	}

	protected override async Task<byte[]> ReceiveBytesInternal(CancellationToken cancellationToken) {
//SystemLog.Info("UDPChannel About to Block Waiting for Connection");
		var result = await _client.ReceiveAsync().WithCancellationToken(cancellationToken);
//SystemLog.Info("UDPChannel Received Data");

		return result.Buffer;
	}

	protected override async Task<bool> TrySendBytesInternal(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken) {
		try {
			SystemLog.Info($"UDP Channel Sending to: {RemoteEndpoint.Address} Port: {RemoteEndpoint.Port}");
			var sentLength = await _client.SendAsync(bytes.ToArray(), bytes.Length).WithCancellationToken(cancellationToken);
			if (sentLength != bytes.Length)
				return false;
		} catch (Exception ex) {
			return false;
		}
		return true;
	}

	public override bool IsConnectionAlive() {
		// UDP has no concept of a connection being alive
		return true;
	}

	public new void Dispose() {
		_client?.Dispose();
	}
}
