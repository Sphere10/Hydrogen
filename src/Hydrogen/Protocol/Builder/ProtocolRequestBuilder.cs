// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Communications;


public class ProtocolRequestBuilder<TRequest>  {

	private IRequestHandler _requestHandler;
	private IResponseHandler _responseHandler;
	

	public ProtocolRequestBuilder<TRequest> HandleRequestWith<TResponse>(Func<TResponse> handler)
		=> HandleRequestWith(_ => handler());

	public ResponseBuilder<TRequest, TResponse> HandleRequestWith<TResponse>(Func<TRequest, TResponse> handler)
		=> HandleRequestWith((_, request) => handler(request));

	public ResponseBuilder<TRequest, TResponse> HandleRequestWith<TResponse>(Func<ProtocolOrchestrator, TRequest, TResponse> handler) 
		=> HandleRequestWith(new ActionRequestHandler<TRequest, TResponse>(handler));

	public ResponseBuilder<TRequest, TResponse> HandleRequestWith<TResponse>(IRequestHandler<TRequest, TResponse>  handler) {
		HandleRequestWith((IRequestHandler)handler);
		return new ResponseBuilder<TRequest, TResponse>(this);
	}

	public ProtocolRequestBuilder<TRequest> HandleRequestWith(IRequestHandler requestHandler) {
		Guard.ArgumentNotNull(requestHandler, nameof(requestHandler));
		Guard.Argument(requestHandler.RequestType == typeof(TRequest), nameof(requestHandler), "Request type mismatch");
		_requestHandler = requestHandler;
		return this;
	}

	public ProtocolRequestBuilder<TRequest> HandleWith(IResponseHandler handler) {
		Guard.ArgumentNotNull(handler, nameof(handler));
		Guard.Argument(handler.RequestType == typeof(TRequest), nameof(handler), "Request type mismatch");
		_responseHandler = handler;
		return this;
	}

	public (IRequestHandler, IResponseHandler) Build() {
		Guard.Ensure(_requestHandler is not null, "Request handler not set");
		Guard.Ensure(_responseHandler is not null, "Response handler not set");
		Guard.Ensure(_requestHandler.RequestType == _responseHandler.RequestType, "Request type mismatch on handlers");
		Guard.Ensure(_requestHandler.ResponseType == _responseHandler.ResponseType, "Response type mismatch on handlers");
		return (_requestHandler, _responseHandler);
	}


	public class ResponseBuilder<TRequest, TResponse> : ProtocolRequestBuilder<TRequest>  {

		private ProtocolRequestBuilder<TRequest> _requestBuilder;

		public ResponseBuilder(ProtocolRequestBuilder<TRequest> requestBuilder) {
			_requestBuilder = requestBuilder;
		}

		public ProtocolRequestBuilder<TRequest> HandleResponseWith(Action<TResponse> handler)
			=> HandleResponseWith((_, response) => handler(response));

		public ProtocolRequestBuilder<TRequest> HandleResponseWith(Action<TRequest, TResponse> handler)
			=> HandleResponseWith((_, request, response) => handler(request, response));

		public ProtocolRequestBuilder<TRequest> HandleResponseWith(Action<ProtocolOrchestrator, TRequest, TResponse> handler)
			=> HandleResponseWith(new ActionResponseHandler<TRequest, TResponse>(handler));

		public ProtocolRequestBuilder<TRequest> HandleResponseWith(IResponseHandler<TRequest, TResponse> handler) 
			=> _requestBuilder.HandleWith(handler);

	}

}
