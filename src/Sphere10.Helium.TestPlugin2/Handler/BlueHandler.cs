using System;
using Sphere10.Helium.Handler;
using Sphere10.Helium.TestPlugin2.Message;

namespace Sphere10.Helium.TestPlugin2.Handler {
	public class BlueHandler : Helium.Handler.Handler, IHandleMessage<BlueHandlerMessage2> {

		public void Handle(BlueHandlerMessage2 message) {
			throw new NotImplementedException();
		}
	}
}
