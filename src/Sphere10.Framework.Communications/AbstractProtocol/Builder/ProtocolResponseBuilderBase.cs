using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications {
    public abstract class ProtocolResponseBuilderBase<TChannel, TProtocolResponseBuilder> 
		where TChannel : ProtocolChannel
		where TProtocolResponseBuilder : ProtocolResponseBuilderBase<TChannel, TProtocolResponseBuilder> {
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
				=> new HandlerBuilder<TRequest, TResponse>(_parent);
		}


		public class HandlerBuilder<TRequest, TResponse> {
			private readonly TProtocolResponseBuilder _parent;

			public HandlerBuilder(TProtocolResponseBuilder parent) {
				_parent = parent;
			}

			public TProtocolResponseBuilder HandleWith(Action<TChannel, TRequest, TResponse> handler) 
				=> HandleWith(new ActionResponseHandler<TChannel, TRequest, TResponse>(handler));

			public TProtocolResponseBuilder HandleWith(IResponseHandler<TChannel, TRequest, TResponse> handler) {
				_parent.ResponseHandlers.Add(typeof(TRequest), typeof(TResponse), handler);
				return _parent;
			}
		}
	
	}
}
