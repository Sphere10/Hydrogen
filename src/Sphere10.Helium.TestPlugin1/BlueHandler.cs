using System;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Handler;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.TestPlugin1 {
	public class BlueHandler : IHandleMessage<BlueHandlerMessage> {
		private readonly IBus _bus;

		public BlueHandler(IBus bus) {
			_bus = bus;
		}

		public void Handle(BlueHandlerMessage message) {
			throw new NotImplementedException();
		}
	}

	public class BlueHandlerMessage : IMessage {
		public string Id { get; set; }
	}
}
