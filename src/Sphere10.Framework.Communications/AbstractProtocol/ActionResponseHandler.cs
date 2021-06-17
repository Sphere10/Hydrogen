using System;

namespace Sphere10.Framework.Communications {
    public class ActionResponseHandler : ResponseHandlerBase {
		private readonly Action<ProtocolChannel, object, object> _action;

		public ActionResponseHandler(Action<ProtocolChannel, object, object> action) {
			Guard.ArgumentNotNull(action, nameof(action));
			_action = action;
		}

		public override void Execute(ProtocolChannel channel, object request, object response) {
			Guard.ArgumentNotNull(channel, nameof(channel));
			Guard.ArgumentNotNull(request, nameof(request));
			Guard.ArgumentNotNull(request, nameof(response));
			_action.Invoke(channel, request, response);
		}
		
	}

}
