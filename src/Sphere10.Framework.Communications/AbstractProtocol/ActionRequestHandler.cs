using System;

namespace Sphere10.Framework.Communications {
    public class ActionRequestHandler : RequestHandlerBase {
		private readonly Func<ProtocolChannel, object, object> _handler;

		public ActionRequestHandler(Func<ProtocolChannel, object, object> handler) {
			_handler = handler;
		}

		public override object Execute(ProtocolChannel channel, object request) {
			return _handler.Invoke(channel, request);
		}
	}

}
