using System;

namespace Sphere10.Framework.Communications {
    public class ActionResponseHandler<TChannel, TRequest, TResponse> : ResponseHandlerBase<TChannel, TRequest, TResponse>
		where TChannel : ProtocolChannel {
		private readonly Action<TChannel, TRequest, TResponse> _handler;

		public ActionResponseHandler(Action<TChannel, TRequest, TResponse> handler) {
			_handler = handler;
		}

		public override void Execute(TChannel channel, TRequest request, TResponse response) => _handler(channel, request, response);

	}

}
