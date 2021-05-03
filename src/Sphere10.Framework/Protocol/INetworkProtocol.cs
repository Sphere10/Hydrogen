//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Sphere10.Framework.Protocol {

//	public interface IProtocolChannel<TEndpoint, TMessageID> {
//		void Initiate(IHandshake handshake);

//		void SendCommand(IProtocolCommand command);

//		void SendRequest(IProtocolRequest request);

//		Func<ResponseResult>? ReceiveCommand(IProtocolCommand command);

//		Func<ResponseResult>? ReceiveHandshake(IHandshake handshake);

//		Func<ResponseResult>? ReceiveResponse(IProtocolRequest request, IProtocolResponse response);

//	}

//	public class ResponseResult {

//	}

//	public interface IProtocolMessage<TEndpoint, TMessageID> {

//		TMessageID ID { get; }

//		TEndpoint Sender { get; }

//		UInt16 MessageType { get; }

//		UInt32 Nonce { get; }

//		byte[] Payload { get; }
//	}

//	public interface IProtocolCommand<TEndpoint, TMessageID> : IProtocolMessage<TEndpoint, TMessageID> {

//	}

//	public interface IProtocolHandshake<TEndpoint, TMessageID> : IProtocolCommand<TEndpoint, TMessageID> {
//	}

	
//	public interface IProtocolRequest<TEndpoint, TMessageID> : IProtocolMessage<TEndpoint, TMessageID> {

//	}

//	public interface IProtocolResponse<TEndpoint, TMessageID> : IProtocolMessage<TEndpoint, TMessageID> {

//		TMessageID RequestID { get; }
//	}

//	public class ProtocolSpecification {

//		RulesSpecification Rules { get; }
//		public class RulesSpecification {

//		}
//	}


//	[Flags]
//	public enum ProtocolOptions {
//		None	     = 0,
//		UseDDoSNonce = 1 << 0, 

//	}

//	public class Foo {

//		public void Bar() {


//			var protocolSpecification = new ProtocolSpecification {

//				Options = ProtocolOptions.UseDDoSNonce,

//				Handshake = new Handshake1();

//				Commands = new[] {
//					new ProtocolCommand1(),
//					new ProtocolCommand2(),
//				},

//				RequestResponses = new [] {
//					new ProtocolRequestResponse(new Request1(), new Response1()),
//					new ProtocolRequestResponse(new Request2(), new Response2()),
//				},

//			}
//		}
//	}

//}
