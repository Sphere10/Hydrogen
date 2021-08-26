using Sphere10.Helium.Message;
using System;
using Sphere10.Helium.Processor;
using Sphere10.Helium.Queue;
using Sphere10.Helium.Timeout;

namespace Sphere10.Helium.Bus {
	public class Bus : IBus {
		private readonly IMessageHeader _messageHeader;
		private readonly ITimeoutManager _timeoutManagerManager;
		private readonly ILocalQueueOutputProcessor _queueOutputManager;

		public Bus(ILocalQueueOutputProcessor queueOutputManager, IMessageHeader messageHeader, ITimeoutManager timeoutManagerManager) {
			_messageHeader = messageHeader;
			_timeoutManagerManager = timeoutManagerManager;
			_queueOutputManager = queueOutputManager;
		}

		public ICallback SendLocal(IMessage message) {
			throw new NotImplementedException();
		}

		public ICallback SendLocal<Tk>(IMessage message, IMessageHeader missingName) {
			throw new NotImplementedException();
		}

		public ICallback RegisterTimeout(TimeSpan delay, IMessage message) {
			throw new NotImplementedException();
		}

		public ICallback RegisterTimeout(DateTime processAt, IMessage message) {
			throw new NotImplementedException();
		}

		public void Reply<Tk>(Action<Tk> messageConstructor) {
			throw new NotImplementedException();
		}

		public void Return<Tk>(Tk errorEnum) {
			throw new NotImplementedException();
		}

		public void SendAndForget(string destination, IMessage message) {
			var headerMessage = _messageHeader.AddHeadersToMessage(message);

			//_queueOutputManager.FirstIn(destination, headerMessage);
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

		public void Subscribe<TK>() {
			throw new NotImplementedException();
		}

		public void Unsubscribe<TK>() {
			throw new NotImplementedException();
		}
	}
}
