using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;
using System;

namespace Sphere10.Helium.Bus {
	public class SendOnlyBus : ISendOnlyBus {
		private readonly IMessageHeader _messageHeader;
		private readonly IQueueManager _queueManager;

		public SendOnlyBus(IQueueManager queueManager, IMessageHeader messageHeader) {
			_messageHeader = messageHeader;
			_queueManager = queueManager;
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
