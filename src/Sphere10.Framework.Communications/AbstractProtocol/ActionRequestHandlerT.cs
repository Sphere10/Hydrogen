using System;

namespace Sphere10.Framework.Communications {
    public class ActionRequestHandler<TChannel, TRequest, TResponse> : RequestHandlerBase<TChannel, TRequest, TResponse> 
		where TChannel : ProtocolChannel {
		private readonly Func<TChannel, TRequest, TResponse> _handler;

		public ActionRequestHandler(Func<TChannel, TRequest, TResponse> handler) {
			_handler = handler;
		}

		public override TResponse Execute(TChannel channel, TRequest request) => _handler(channel, request);

	}

}
