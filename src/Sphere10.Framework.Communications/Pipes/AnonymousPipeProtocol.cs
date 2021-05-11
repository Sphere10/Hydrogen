using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using Sphere10.Framework.Communications.Protocol;

namespace Sphere10.Framework {


	public class AnonymousPipeProtocol<TMessageType, TCommandHandler, TRequestHandler> : ProtocolBase<string, TMessageType, AnonymousPipeProtocol<TMessageType, TCommandHandler, TRequestHandler>.Message, TCommandHandler, TRequestHandler> 
		where TMessageType : Enum
		where TCommandHandler : ICommandHandler<string, AnonymousPipeProtocol<TMessageType, TCommandHandler, TRequestHandler>.Message>
		where TRequestHandler : IRequestHandler<string, AnonymousPipeProtocol<TMessageType, TCommandHandler, TRequestHandler>.Message> {

		public class Message {
		}

		public class Handshake : Message {
			public string PipeHandle { get; init; }
		}

		public class AnonymousPipeHub : ProtocolHubBase<Channel, AnonymousPipeProtocol<TMessageType, TCommandHandler, TRequestHandler>> {

			public override Channel InitiateConnection(string endpoint, Message handshake) {
				throw new NotImplementedException();
			}

			public override void Run(AnonymousPipeProtocol<TMessageType, TCommandHandler, TRequestHandler> protocol, CancellationToken cancellationToken) {
				throw new NotImplementedException();
			}
		}

		public class Channel : ChannelBase {
			private AnonymousPipeClientStream _pipeClient;
			private StreamReader _reader;
			private StreamWriter _writer;

			public IItemSerializer<Message> MessageSerializer { get; init; }

			public override void Open(Message message) {
				Guard.ArgumentCast<Handshake>(message, out var handshake, nameof(message));
				_pipeClient = new AnonymousPipeClientStream(PipeDirection.InOut, handshake.PipeHandle);
				_reader = new StreamReader(_pipeClient);
				_writer = new StreamWriter(_pipeClient);
			}

			public override void Close() {
				_writer.Flush();
				_pipeClient.Flush();
				_reader.Dispose();
				_writer.Dispose();
				_pipeClient.Dispose();
			}

			public override void SendMessage(TMessageType messageType, Message message) {
				Guard.ArgumentCast<ushort>(messageType, out var messageTypeVar, nameof(messageType));
				_writer.Write(message);
			}

			protected override void FreeManagedResources() {
				throw new NotImplementedException();
			}


		}

	}

	//public class AnonymousPipeHub<TMessageType> : ProtocolHubBase<string, uint, TMessageType, uint, string, AnonymousPipeHub<TMessageType>.AnonymousPipeMessage, AnonymousPipeHub<TMessageType>.AnonymousPipeCommand, AnonymousPipeHub<TMessageType>.AnonymousPipeRequest, AnonymousPipeHub<TMessageType>.AnonymousPipeResponse,
	//	AnonymousPipeHub<TMessageType>.AnonymousPipeHandshake, AnonymousPipeHub<TMessageType>.AnonymousPipeChannel> {
	//	public override AnonymousPipeChannel InitiateConnection(AnonymousPipeHandshake handshake) {
	//		throw new NotImplementedException();
	//	}

	//	public override AnonymousPipeChannel ReceiveConnection(AnonymousPipeHandshake handshake) {
	//		throw new NotImplementedException();
	//	}


	//	public class AnonymousPipeChannel : ProtocolChannelBase<string, uint, TMessageType, uint, string, AnonymousPipeMessage, AnonymousPipeCommand, AnonymousPipeRequest, AnonymousPipeResponse, AnonymousPipeHandshake> {
	//		private AnonymousPipeClientStream _pipeClient;
	//		private StreamReader _reader;
	//		private StreamWriter _writer;


	//		public AnonymousPipeChannel() {
	//		}

	//		public override void Open(AnonymousPipeHandshake handshake) {
	//			_pipeClient = new AnonymousPipeClientStream(PipeDirection.InOut, handshake.PipeHandle);
	//			_reader = new StreamReader(_pipeClient);
	//			_writer = new StreamWriter(_pipeClient);
	//		}

	//		public override void Close() {
	//			// close & dispose
	//			_pipeClient?.Close();
	//			_reader?.Close();
	//			_writer?.Close();

	//			_writer?.Dispose();
	//			_reader?.Dispose();
	//			_pipeClient?.Dispose();

	//		}

	//		public override void SendMessage(AnonymousPipeMessage message) {
	//			_writer.WriteLine
	//		}

	//		public override void SendCommand(AnonymousPipeCommand command) {
	//			throw new NotImplementedException();
	//		}

	//		public override void SendRequest(AnonymousPipeRequest request) {
	//			throw new NotImplementedException();
	//		}

	//		public override void ReceiveCommand(AnonymousPipeCommand command) {
	//			throw new NotImplementedException();
	//		}

	//		public override AnonymousPipeResponse ReceiveRequest(AnonymousPipeRequest request) {
	//			throw new NotImplementedException();
	//		}

	//		public override void ReceiveResponse(AnonymousPipeRequest sentRequest, AnonymousPipeResponse response) {
	//			throw new NotImplementedException();
	//		}

	//		protected override void FreeManagedResources() {
	//			throw new NotImplementedException();
	//		}
	//	}

	//	public class AnonymousPipeMessage : ProtocolMessageBase<string, uint, TMessageType, uint, string> {
	//	}

	//	public class AnonymousPipeCommand : ProtocolCommand<string, uint, TMessageType, uint, string> {
	//	}

	//	public class AnonymousPipeHandshake : ProtocolHandshake<string, uint, TMessageType, uint, string> {
	//		public string PipeHandle { get; init; }
	//	}

	//	public class AnonymousPipeRequest : ProtocolRequestBase<string, uint, TMessageType, uint, string> {
	//	}

	//	public class AnonymousPipeResponse : ProtocolResponse<string, uint, TMessageType, uint, string> {
	//	}
	//}
}
