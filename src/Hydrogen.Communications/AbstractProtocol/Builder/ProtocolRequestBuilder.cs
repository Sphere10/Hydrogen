using System;
using System.Collections.Generic;

namespace Hydrogen.Communications {
    public class ProtocolRequestBuilder : ProtocolBuilderMain {
		ProtocolMode _mode;

		public ProtocolRequestBuilder(ProtocolModeBuilder parent, ProtocolMode mode)
			: base(parent) {
			_mode = mode;
		}

		public HandlerBuilder<TRequest> ForRequest<TRequest>() {
			return new(this);
		}

		protected void Add(Type type, IRequestHandler handler) {
			_mode.RequestHandlers.Add(type, handler);
        }

		public class HandlerBuilder<TRequest> {
			private readonly ProtocolRequestBuilder _parent;

			public HandlerBuilder(ProtocolRequestBuilder parent) {
				_parent = parent;
			}

			public ProtocolRequestBuilder RespondWith<TResponse>(Func<TResponse> handler)
				=> RespondWith(_ => handler());

			public ProtocolRequestBuilder RespondWith<TResponse>(Func<TRequest, TResponse> handler)
				=> RespondWith((_, request) => handler(request));

			public ProtocolRequestBuilder RespondWith<TResponse>(Func<ProtocolOrchestrator, TRequest, TResponse> handler) {
				_parent._mode.RequestHandlers.Add(typeof(TRequest), new ActionRequestHandler<TRequest, TResponse>(handler));
				return _parent;
			}
		}
	}

}
