// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Communications;

public abstract class ResponseHandlerBase<TRequest, TResponse> : ResponseHandlerBase, IResponseHandler<TRequest, TResponse> {
	public sealed override void Execute(ProtocolOrchestrator orchestrator, object request, object response) {
		Guard.ArgumentCast<TRequest>(request, out var requestT, nameof(request));
		Guard.ArgumentCast<TResponse>(response, out var responseT, nameof(response));
		Execute(orchestrator, requestT, responseT);
	}

	public abstract void Execute(ProtocolOrchestrator orchestrator, TRequest request, TResponse response);

}
