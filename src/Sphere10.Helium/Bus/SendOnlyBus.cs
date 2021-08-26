using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;
using System;
using Sphere10.Helium.Processor;

namespace Sphere10.Helium.Bus {
	public class SendOnlyBus : ISendOnlyBus {
		private readonly IMessageHeader _messageHeader;
		private readonly ILocalQueueOutputProcessor _queueOutputManager;

		public SendOnlyBus(ILocalQueueOutputProcessor queueOutputManager, IMessageHeader messageHeader) {
			_messageHeader = messageHeader;
			_queueOutputManager = queueOutputManager;
		}

		public void Dispose() {
			throw new NotImplementedException();
		}

		public void SendAndForget(string destination, IMessage message) {
			throw new NotImplementedException();
		}

		public void SendAndForget(string destination, IMessage message, IMessageHeader messageHeader) {
			throw new NotImplementedException();
		}

		public ICallback SendAndResponse(string destination, IMessage message) {
			throw new NotImplementedException();
		}

		public ICallback SendAndResponse(string destination, IMessage message, IMessageHeader messageHeader) {
			throw new NotImplementedException();
		}
	}
}
