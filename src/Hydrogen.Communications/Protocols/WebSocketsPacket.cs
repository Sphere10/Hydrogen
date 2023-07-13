// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;
using Newtonsoft.Json;

namespace Hydrogen.Communications;

public class WebSocketsPacket {
	public string Id { get; set; }
	public string Message { get; set; }
	public string[] Tokens { get; private set; } = new string[0];
	public string JsonData { get; set; }

	public WebSocketsPacket() {
	}

	public WebSocketsPacket(byte[] bytes) {

		var text = Encoding.ASCII.GetString(bytes);
		WebSocketsPacket packet = JsonConvert.DeserializeObject<WebSocketsPacket>(text);

		Id = packet.Id;
		Message = packet.Message;
		Tokens = Message.Split(" ");
		JsonData = packet.JsonData;
	}

	public WebSocketsPacket(string id, string message, string jsonData) {
		Id = id;
		Message = message;
		Tokens = Message.Split(" ");
		JsonData = jsonData;
	}

	public WebSocketsPacket(string id, string message) {
		Id = id;
		Message = message;
		Tokens = Message.Split(" ");
	}

	public byte[] ToBytes() {
		var jsonData = JsonConvert.SerializeObject(this);
		return Encoding.ASCII.GetBytes(jsonData);
	}
}
