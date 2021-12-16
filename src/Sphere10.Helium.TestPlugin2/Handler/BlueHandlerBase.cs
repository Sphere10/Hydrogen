using System;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Handle;
using Sphere10.Helium.TestPlugin2.Message;

namespace Sphere10.Helium.TestPlugin2.Handler {
	public class BlueHandlerBase : HandlerBase, IHandleMessage<BlueHandlerMessage2> {

		public BlueHandlerBase(IBus bus) : base(bus) {
		}

		public void Handle(BlueHandlerMessage2 message) {
			throw new NotImplementedException();
		}
	}
}