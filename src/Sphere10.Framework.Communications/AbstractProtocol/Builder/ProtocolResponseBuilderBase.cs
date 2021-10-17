using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications {
    public abstract class ProtocolResponseBuilderBase<TProtocolResponseBuilder> 
		where TProtocolResponseBuilder : ProtocolResponseBuilderBase<TProtocolResponseBuilder> {
		protected readonly MultiKeyDictionary<Type, Type, IResponseHandler> ResponseHandlers;

		protected ProtocolResponseBuilderBase() 
			: this(new MultiKeyDictionary<Type, Type, IResponseHandler>()) {
        }

		protected ProtocolResponseBuilderBase(MultiKeyDictionary<Type, Type, IResponseHandler> responseHandlers) {
			ResponseHandlers = responseHandlers;
		}

		public FromRequestBuilder<TResponse> ForResponse<TResponse>() {
			return new FromRequestBuilder<TResponse>(this as TProtocolResponseBuilder);
		}

		protected void Add(Type requestMessageType, Type responseMessageType, IResponseHandler handler) {
			ResponseHandlers.Add(requestMessageType, responseMessageType, handler);
        }

		public class FromRequestBuilder<TResponse> {
			private readonly TProtocolResponseBuilder _parent;

			public FromRequestBuilder(TProtocolResponseBuilder parent) {
				_parent = parent;
			}

			public HandlerBuilder<TRequest, TResponse> ToRequest<TRequest>() 
				=> new(_parent);
		}

		public class HandlerBuilder<TRequest, TResponse> {
			private readonly TProtocolResponseBuilder _parent;

			public HandlerBuilder(TProtocolResponseBuilder parent) {
				_parent = parent;
			}

			public TProtocolResponseBuilder HandleWith(Action handler)
				=> HandleWith(_ => handler());

			public TProtocolResponseBuilder HandleWith(Action<TResponse> handler)
				=> HandleWith((_, response) => handler(response));

			public TProtocolResponseBuilder HandleWith(Action<TRequest, TResponse> handler)
				=> HandleWith((_, request, response) => handler(request, response));
			
			public TProtocolResponseBuilder HandleWith(Action<ProtocolOrchestrator, TRequest, TResponse> handler)
				=> HandleWith(new ActionResponseHandler<TRequest, TResponse>(handler));

			public TProtocolResponseBuilder HandleWith(IResponseHandler<TRequest, TResponse> handler) {
				_parent.ResponseHandlers.Add(typeof(TRequest), typeof(TResponse), handler);
				return _parent;
			}
		}
	
	}
}
