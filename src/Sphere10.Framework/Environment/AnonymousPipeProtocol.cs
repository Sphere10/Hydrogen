using System;
using System.Collections.Generic;
using System.Text;
using Sphere10.Framework.Protocol;

namespace Sphere10.Framework {

	public class AnonymousPipeHub<TMessageType> : ProtocolHub<string, uint, TMessageType, uint, string, AnonymousPipeHub<TMessageType>.AnonymousPipeMessage, AnonymousPipeHub<TMessageType>.AnonymousPipeCommand, AnonymousPipeHub<TMessageType>.AnonymousPipeRequest, AnonymousPipeHub<TMessageType>.AnonymousPipeResponse,
		AnonymousPipeHub<TMessageType>.AnonymousPipeHandshake, AnonymousPipeHub<TMessageType>.AnonymousPipeChannel> {

		public override AnonymousPipeChannel ReceiveConnection(AnonymousPipeHandshake handshake) {
			throw new NotImplementedException();
		}

		public override AnonymousPipeChannel InitiateConnection(AnonymousPipeHandshake handshake) {
			throw new NotImplementedException();
		}

		public class AnonymousPipeChannel : ProtocolChannel<string, uint, TMessageType, uint, string, AnonymousPipeMessage, AnonymousPipeCommand, AnonymousPipeRequest,
		AnonymousPipeResponse> {

			public override void SendMessage(AnonymousPipeMessage message) {
				throw new NotImplementedException();
			}

			public override void SendCommand(AnonymousPipeCommand command) {
				throw new NotImplementedException();
			}

			public override void SendRequest(AnonymousPipeRequest request) {
				throw new NotImplementedException();
			}

			public override void ReceiveCommand(AnonymousPipeCommand command) {
				throw new NotImplementedException();
			}

			public override AnonymousPipeResponse ReceiveRequest(AnonymousPipeRequest request) {
				throw new NotImplementedException();
			}

			public override void ReceiveResponse(AnonymousPipeRequest sentRequest, AnonymousPipeResponse response) {
				throw new NotImplementedException();
			}
		}

		public class AnonymousPipeMessage : ProtocolMessage<string, uint, TMessageType, uint, string> {
		}

		public class AnonymousPipeCommand : ProtocolCommand<string, uint, TMessageType, uint, string> {
		}

		public class AnonymousPipeHandshake : ProtocolHandshake<string, uint, TMessageType, uint, string> {
		}

		public class AnonymousPipeRequest : ProtocolRequest<string, uint, TMessageType, uint, string> {
		}

		public class AnonymousPipeResponse : ProtocolResponse<string, uint, TMessageType, uint, string> {
		}
	}
}
