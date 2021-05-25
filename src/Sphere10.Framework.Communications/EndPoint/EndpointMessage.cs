using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Framework.Communications.RPC {
	//Implement a communication endpoint message with bytes or strings
	public class EndpointMessage {
		public byte[] messageData;
		public IEndPoint stream;

		public EndpointMessage(string message, IEndPoint targetStream = null) {
			FromString(message);
			stream = targetStream;
		}
		public EndpointMessage(byte[] message, IEndPoint targetStream = null) {
			messageData = message;
			stream = targetStream;
		}
		public override string ToString() {
			ASCIIEncoding encoder = new ASCIIEncoding();
			return encoder.GetString(messageData, 0, messageData.Length);
		}
		public void FromString(string message) {
			ASCIIEncoding encoder = new ASCIIEncoding();
			messageData = encoder.GetBytes(message);
		}
	}
}