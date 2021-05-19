using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Framework.Communications.RPC {
	//Implement a communication endpoint message with bytes or strings
	public class EndpointMessage {
		public byte[] messageData;
		public IEndPoint streamContext;

		public EndpointMessage(string message, IEndPoint stream = null) {
			FromString(message);
			streamContext = stream;
		}
		public EndpointMessage(byte[] message, IEndPoint stream = null) {
			messageData = message;
			streamContext = stream;
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