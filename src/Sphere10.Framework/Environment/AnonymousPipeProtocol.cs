using System;
using System.Collections.Generic;
using System.Text;
using Sphere10.Framework.Protocol;

namespace Sphere10.Framework {

	public class AnonymousPipeHub<TMessageType> : ProtocolHub<string, uint, TMessageType, uint, string, AnonymousPipeMessage<TMessageType>, AnonymousPipeCommand<TMessageType>, AnonymousPipeRequest<TMessageType>, AnonymousPipeResponse<TMessageType>,
		AnonymousPipeHandshake<TMessageType>, AnonymousPipeChannel<TMessageType>> {

		public override AnonymousPipeChannel<TMessageType> ReceiveConnection(AnonymousPipeHandshake<TMessageType> handshake) {
			throw new NotImplementedException();
		}

		public override AnonymousPipeChannel<TMessageType> InitiateConnection(AnonymousPipeHandshake<TMessageType> handshake) {
			throw new NotImplementedException();
		}
	}


	public class AnonymousPipeChannel<TMessageType> : ProtocolChannel<string, uint, TMessageType, uint, string, AnonymousPipeMessage<TMessageType>, AnonymousPipeCommand<TMessageType>, AnonymousPipeRequest<TMessageType>,
		AnonymousPipeResponse<TMessageType>> {

		public override void SendMessage(AnonymousPipeMessage<TMessageType> message) {
			throw new NotImplementedException();
		}

		public override void SendCommand(AnonymousPipeCommand<TMessageType> command) {
			throw new NotImplementedException();
		}

		public override void SendRequest(AnonymousPipeRequest<TMessageType> request) {
			throw new NotImplementedException();
		}

		public override void ReceiveCommand(AnonymousPipeCommand<TMessageType> command) {
			throw new NotImplementedException();
		}

		public override AnonymousPipeResponse<TMessageType> ReceiveRequest(AnonymousPipeRequest<TMessageType> request) {
			throw new NotImplementedException();
		}

		public override void ReceiveResponse(AnonymousPipeRequest<TMessageType> sentRequest, AnonymousPipeResponse<TMessageType> response) {
			throw new NotImplementedException();
		}
	}

	public class AnonymousPipeMessage<TMessageType> : ProtocolMessage<string, uint, TMessageType, uint, string> {
	}


	public class AnonymousPipeCommand<TMessageType> : ProtocolCommand<string, uint, TMessageType, uint, string> {
	}

	public class AnonymousPipeHandshake<TMessageType> : ProtocolHandshake<string, uint, TMessageType, uint, string> {
	}

	public class AnonymousPipeRequest<TMessageType> : ProtocolRequest<string, uint, TMessageType, uint, string> {
	}

	public class AnonymousPipeResponse<TMessageType> : ProtocolResponse<string, uint, TMessageType, uint, string> {
	}


}
