using System;

namespace Sphere10.Framework.Communications {
    public class ActionResponseHandler<TChannel, TRequest, TResponse> : ResponseHandlerBase<TChannel, TRequest, TResponse>
		where TChannel : ProtocolChannel {
		private readonly Action<TChannel, TRequest, TResponse> _action;

		public ActionResponseHandler(Action<TChannel, TRequest, TResponse> action) {
			Guard.ArgumentNotNull(action, nameof(action));
			_action = action;
		}

		public override void Execute(TChannel channel, TRequest request, TResponse response) {
			Guard.ArgumentNotNull(channel, nameof(channel));
			Guard.ArgumentNotNull(request, nameof(request));
			Guard.ArgumentNotNull(request, nameof(response));
			_action(channel, request, response);
		}

	}

}
