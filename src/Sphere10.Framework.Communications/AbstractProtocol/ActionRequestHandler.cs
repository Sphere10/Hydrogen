using System;

namespace Sphere10.Framework.Communications {
    public class ActionRequestHandler : RequestHandlerBase {
		private readonly Func<ProtocolChannel, object, object> _action;

		public ActionRequestHandler(Func<ProtocolChannel, object, object> action) {
			Guard.ArgumentNotNull(action, nameof(action));
			_action = action;
		}

		public override object Execute(ProtocolChannel channel, object request) {
			Guard.ArgumentNotNull(channel, nameof(channel));
			Guard.ArgumentNotNull(request, nameof(request));
			return _action(channel, request);
		}
	}

}
