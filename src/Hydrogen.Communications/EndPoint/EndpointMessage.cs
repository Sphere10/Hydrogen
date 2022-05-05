﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen.Communications.RPC {
	//Implement a communication endpoint message with bytes or strings
	public class EndpointMessage {
		public byte[] MessageData;
		public IEndPoint Stream;

		public EndpointMessage(string message, IEndPoint targetStream = null) {
			FromString(message);
			Stream = targetStream;
		}
		public EndpointMessage(byte[] message, IEndPoint targetStream = null) {
			MessageData = message;
			Stream = targetStream;
		}
		public string ToSafeString() {
			ASCIIEncoding encoder = new ASCIIEncoding();
			var safeArray = TcpSecurityPolicies.RemoveControlCharacters(MessageData, MessageData.Length);
			var trimed = encoder.GetString(MessageData, 0, MessageData.Length).Trim();
			return trimed;
		}
		public override string ToString() {
			ASCIIEncoding encoder = new ASCIIEncoding();
			return encoder.GetString(MessageData, 0, MessageData.Length);
		}
		public void FromString(string message) {
			ASCIIEncoding encoder = new ASCIIEncoding();
			MessageData = encoder.GetBytes(message);
		}
	}
}