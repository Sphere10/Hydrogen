using System;

namespace Sphere10.Framework.Communications {
    public class ActionRequestHandler<TChannel, TRequest, TResponse> : RequestHandlerBase<TChannel, TRequest, TResponse> 
		where TChannel : ProtocolChannel {
		private readonly Func<TChannel, TRequest, TResponse> _action;

		public ActionRequestHandler(Func<TChannel, TRequest, TResponse> action) {
			Guard.ArgumentNotNull(action, nameof(action));
			_action = action;
		}

		public override TResponse Execute(TChannel channel, TRequest request) {
			Guard.ArgumentNotNull(channel, nameof(channel));
			Guard.ArgumentNotNull(request, nameof(request));
			return _action(channel, request);
		}

	}

}
