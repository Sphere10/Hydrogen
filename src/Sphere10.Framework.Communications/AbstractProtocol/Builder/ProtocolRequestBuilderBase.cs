using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications {
    public abstract class ProtocolRequestBuilderBase<TProtocolRequestBuilder> 
		where TProtocolRequestBuilder : ProtocolRequestBuilderBase<TProtocolRequestBuilder> {
		protected readonly IDictionary<Type, IRequestHandler> RequestHandlers;

		protected ProtocolRequestBuilderBase() 
			: this(new Dictionary<Type, IRequestHandler>()) {
        }

		protected ProtocolRequestBuilderBase(IDictionary<Type, IRequestHandler> requestHandlers) {
			RequestHandlers = requestHandlers;
		}

		public HandlerBuilder<TRequest> ForRequest<TRequest>() {
			return new HandlerBuilder<TRequest>(this as TProtocolRequestBuilder);
		}

		protected void Add(Type type, IRequestHandler handler) {
			RequestHandlers.Add(type, handler);
        }

		public class HandlerBuilder<TRequest> {
			private readonly TProtocolRequestBuilder _parent;

			public HandlerBuilder(TProtocolRequestBuilder parent) {
				_parent = parent;
			}

			public TProtocolRequestBuilder RespondWith<TResponse>(Func<TResponse> handler)
				=> RespondWith(_ => handler());

			public TProtocolRequestBuilder RespondWith<TResponse>(Func<TRequest, TResponse> handler)
				=> RespondWith((_, request) => handler(request));

			public TProtocolRequestBuilder RespondWith<TResponse>(Func<ProtocolOrchestrator, TRequest, TResponse> handler) {
				_parent.RequestHandlers.Add(typeof(TRequest), new ActionRequestHandler<TRequest, TResponse>(handler));
				return _parent;
			}
		}
	}

}
