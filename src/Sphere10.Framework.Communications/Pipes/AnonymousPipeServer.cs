using System;
using System.IO.Pipes;

namespace Sphere10.Framework.Communications {
	public class AnonymousPipeServer : IChannelMediator {
		private AnonymousPipeClientStream _writePipe;
		private AnonymousPipeClientStream _readPipe;
		private IChannelMediator _channelMediatorImplementation;
		private FactorySerializer<IAnonymousPipeMessage> _serializer;

		public AnonymousPipeServer(FactorySerializer<IAnonymousPipeMessage> serializer) {
			_serializer = serializer;
		}

		public AnonymousPipeChannel InitiateConnection(AnonymousPipeEndpoint serverEndpoint) {
			_writePipe = new AnonymousPipeClientStream(PipeDirection.In, serverEndpoint.ReaderHandle);
			_readPipe = new AnonymousPipeClientStream(PipeDirection.In, serverEndpoint.WriterHandle);
			return new AnonymousPipeChannel(_writePipe, _readPipe, _serializer, CommunicationRole.Client, CommunicationRole.Client, this);
		}

		public void ReportBadData(BadDataType badDataType, AnonymousPipeChannel channel, string additionData = null) {

		}

	}
}
