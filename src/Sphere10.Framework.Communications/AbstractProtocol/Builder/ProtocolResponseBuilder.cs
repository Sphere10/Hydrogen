using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications {

	public class ProtocolResponseBuilder : ProtocolBuilderMain {
		ProtocolMode _mode;

		public ProtocolResponseBuilder(ProtocolModeBuilder parent, ProtocolMode mode)
			: base(parent) {
			_mode = mode;
		}

		public FromRequestBuilder<TResponse> ForResponse<TResponse>() {
			return new FromRequestBuilder<TResponse>(this);
		}

		protected void Add(Type requestMessageType, Type responseMessageType, IResponseHandler handler) {
			_mode.ResponseHandlers.Add(requestMessageType, responseMessageType, handler);
        }

		public class FromRequestBuilder<TResponse> {
			private readonly ProtocolResponseBuilder _parent;

			public FromRequestBuilder(ProtocolResponseBuilder parent) {
				_parent = parent;
			}

			public HandlerBuilder<TRequest, TResponse> ToRequest<TRequest>() 
				=> new(_parent);
		}

		public class HandlerBuilder<TRequest, TResponse> {
			private readonly ProtocolResponseBuilder _parent;

			public HandlerBuilder(ProtocolResponseBuilder parent) {
				_parent = parent;
			}

			public ProtocolResponseBuilder HandleWith(Action handler)
				=> HandleWith(_ => handler());

			public ProtocolResponseBuilder HandleWith(Action<TResponse> handler)
				=> HandleWith((_, response) => handler(response));

			public ProtocolResponseBuilder HandleWith(Action<TRequest, TResponse> handler)
				=> HandleWith((_, request, response) => handler(request, response));

			public ProtocolResponseBuilder HandleWith(Action<ProtocolOrchestrator, TRequest, TResponse> handler)
				=> HandleWith(new ActionResponseHandler<TRequest, TResponse>(handler));

			public ProtocolResponseBuilder HandleWith(IResponseHandler<TRequest, TResponse> handler) {
				_parent.Add(typeof(TRequest), typeof(TResponse), handler);
				return _parent;
			}

		}

    }
}
