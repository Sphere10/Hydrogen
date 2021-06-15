using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Framework.Communications.RPC {
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