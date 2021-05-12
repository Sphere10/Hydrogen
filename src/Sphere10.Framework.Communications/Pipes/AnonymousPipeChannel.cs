using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using Sphere10.Framework.Communications.Protocol;
using Sphere10.Framework;
using Sphere10.Framework.Scheduler;

namespace Sphere10.Framework.Communications {


	public class AnonymousPipeChannel : IDisposable {
		public const int MinMessageLength = 0;
		public const int MaxMessageLength = 1 << 16; // 65k
		private readonly Stream _writeStream;
		private readonly Stream _readStream;
		private readonly EndianBinaryReader _reader;
		private readonly EndianBinaryWriter _writer;
		private readonly Scheduler.Scheduler _scheduler;

		public event EventHandlerEx<object, IAnonymousPipeMessage> ReceivedMessage;

		public AnonymousPipeChannel(Stream writeStream, Stream readStream, FactorySerializer<IAnonymousPipeMessage> messageSerializer, CommunicationRole localRole, CommunicationRole initiator, IChannelMediator channelMediator) {
			_writeStream = writeStream;
			_readStream = readStream;
			_writer = new EndianBinaryWriter(EndianBitConverter.Little, _writeStream);
			_reader = new EndianBinaryReader(EndianBitConverter.Little, _readStream);
			_scheduler = new Scheduler.Scheduler(SchedulerPolicy.ForceSyncronous);
			MessageSerializer = messageSerializer;
			CheckReceivedEvery = TimeSpan.FromMilliseconds(100);
			LocalRole = localRole;
			Initiator = initiator;
			Mediator = channelMediator;
			_scheduler.AddJob(
				JobBuilder
					.For(CheckReceivedMessage)
					.Repeat
					.OnInterval(CheckReceivedEvery)
					.Build()
			);
		}

		public IChannelMediator Mediator { get; init; }

		public CommunicationRole LocalRole { get; init; }

		public CommunicationRole Initiator { get; init; }

		public TimeSpan CheckReceivedEvery { get; init; }

		protected IItemSerializer<IAnonymousPipeMessage> MessageSerializer { get; }

		public void Open() {
			_scheduler.Start();
		}

		public void Close() {
			_scheduler.Stop();
		}

		public void SendMessage(IAnonymousPipeMessage message) {
			MessageSerializer.Serialize(message, _writer);
		}

		public void OnReceivedMessage(IAnonymousPipeMessage message) {
		}

		protected void NotifyReceivedMessage(IAnonymousPipeMessage message) {
			OnReceivedMessage(message);
			ReceivedMessage?.Invoke(this, message);
		}

		public void Dispose() {
			_writeStream?.Dispose();
			_readStream?.Dispose();
			_reader?.Dispose();
			_writer?.Dispose();
			_scheduler?.Dispose();
		}

		private void CheckReceivedMessage() {
			if (_readStream.Length > 0) {
				if (_readStream.Length < 4) {
					Mediator.ReportBadData(BadDataType.MissingMessageLength, this);
					Close();
					return;
				}
				var size = _reader.ReadUInt32();
				if (size < MinMessageLength || size > MaxMessageLength) {
					Mediator.ReportBadData(BadDataType.InvalidMessageLength, this);
					Close();
					return;
				}
				IAnonymousPipeMessage message;
				try {
					message = MessageSerializer.Deserialize((int)size, _reader);
				} catch (Exception error) {
					Mediator.ReportBadData(BadDataType.InvalidMessage, this, error.ToDiagnosticString());
					Close();
					return;
				}
				NotifyReceivedMessage(message);
			}
		}
	}


	//public class AnonymousPipeProtocol<TMessageType, TCommandHandler, TRequestHandler> : ProtocolBase<string, TMessageType, AnonymousPipeProtocol<TMessageType, TCommandHandler, TRequestHandler>.Message, TCommandHandler, TRequestHandler> 
	//	where TMessageType : Enum
	//	where TCommandHandler : ICommandHandler<string, AnonymousPipeProtocol<TMessageType, TCommandHandler, TRequestHandler>.Message>
	//	where TRequestHandler : IRequestHandler<string, AnonymousPipeProtocol<TMessageType, TCommandHandler, TRequestHandler>.Message> {

	//	public class Message {
	//	}

	//	public class Handshake : Message {
	//		public string PipeHandle { get; init; }
	//	}

	//	public class AnonymousPipeHub : ProtocolHubBase<Channel, AnonymousPipeProtocol<TMessageType, TCommandHandler, TRequestHandler>> {
	//		private AnonymousPipeServerStream _pipeServer;
	//		private EndianBinaryReader _reader;
	//		private EndianBinaryWriter _writer;

	//		public AnonymousPipeHub(string childProcess, string handleArgumentSwitch = "", string otherArgs = "") {
	//			_pipeServer = new AnonymousPipeServerStream(PipeDirection.InOut, HandleInheritability.Inheritable);
	//			HubEndpoint = _pipeServer.GetClientHandleAsString();
	//		}

	//		public override Channel InitiateConnection(string endpoint, Message handshake) {
	//			// Anonoymous pipes are between a parent/child processes
	//			throw new NotSupportedException();
	//		}

	//		public override void Run(AnonymousPipeProtocol<TMessageType, TCommandHandler, TRequestHandler> protocol, CancellationToken cancellationToken) {
	//		}
	//	}

	//	public class Channel : ChannelBase {
	//		private AnonymousPipeClientStream _pipeClient;
	//		private EndianBinaryReader _reader;
	//		private EndianBinaryWriter _writer;

	//		public IItemSerializer<Message> MessageSerializer { get; init; }

	//		public override void Open(Message message) {
	//			Guard.ArgumentCast<Handshake>(message, out var handshake, nameof(message));
	//			_pipeClient = new AnonymousPipeClientStream(PipeDirection.InOut, handshake.PipeHandle);
	//			_reader = new EndianBinaryReader(EndianBitConverter.Little, _pipeClient);
	//			_writer = new EndianBinaryWriter(EndianBitConverter.Little, _pipeClient);
	//		}

	//		public override void Close() {
	//			_writer.Flush();
	//			_pipeClient.Flush();
	//		}

	//		public override void SendMessage(TMessageType messageType, Message message) {
	//			Guard.ArgumentCast<ushort>(messageType, out var messageTypeVal, nameof(messageType));
	//			_writer.Write(messageTypeVal);
	//			MessageSerializer.Serialize(message, _writer);
	//		}

	//		protected override void FreeManagedResources() {
	//			_reader.Dispose();
	//			_writer.Dispose();
	//			_pipeClient.Dispose();
	//		}

	//	}

	//}


}
