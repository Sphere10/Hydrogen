// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: David Price
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Text;
using System.Collections.Generic;

namespace Hydrogen.Communications;

public class ServerWebSocketsChannelHub {

	private Dictionary<string, ServerWebSocketsChannel> Channels { get; set; } = new Dictionary<string, ServerWebSocketsChannel>();

	public event EventHandlerEx<ReadOnlyMemory<byte>> ReceivedBytes;
	public ServerWebSocketsChannelHub(ServerWebSocketsChannel channel) {
		Channels.Add(channel.InternalId, channel);
		channel.Closing += Channel_Closing;
		channel.Closed += Channel_Closed;
	}

	private void Channel_Closed() {
//			throw new NotImplementedException();




	}

	private void Channel_Closing() {
//			throw new NotImplementedException();
	}

	int ReportId = 1;
	public string Report() {
		var stringBuilder = new StringBuilder();
		foreach (var channel in Channels) {
			var value = channel.Value;
			stringBuilder.AppendLine($"Key: {channel.Key} State: {value.State} Connection: {value.IsConnectionAlive()}");
		}
		stringBuilder.AppendLine($"ReportId: {ReportId++}");

		return stringBuilder.ToString();
	}

	async public void CloseConnection(string id) {
		if (Channels.ContainsKey(id)) {
			await Channels[id].Close();
		}
	}

	public void Close() {
		foreach (var channel in Channels) {
			channel.Value.Close();
		}
		Channels.Clear();
	}

	public async void ConnectionMade(ServerWebSocketsChannel channel) {
		var newChannel = new ServerWebSocketsChannel(channel, this);
		Channels.Add(newChannel.InternalId, newChannel);
		await newChannel.Open();
		SystemLog.Info($"HUB ConnectionMade() - Id: {newChannel.InternalId}");
	}

	public void ReceivedBytesExternal(ReadOnlyMemory<byte> buffer) {
		var packet = new WebSocketsPacket(buffer.ToArray());
		if (Channels.ContainsKey(packet.Id)) {
			ReceivedBytes?.Invoke(buffer);
		} else {
			SystemLog.Info($"Hub ERROR received data from bad Id {packet.Id} ");
		}
	}

	public void TrySendBytes(WebSocketsPacket packet, bool all) {

		if (all) {
			foreach (var channel in Channels) {
				packet.Id = channel.Key;
				Channels[packet.Id].TrySendBytes(packet.ToBytes());
			}
		} else {
			if (Channels.ContainsKey(packet.Id)) {
				Channels[packet.Id].TrySendBytes(packet.ToBytes());
			} else {
				SystemLog.Info($"Hub ERROR trying to send to bad Id {packet.Id} ");
			}
		}
	}
}
