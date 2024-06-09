// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Communications;

public abstract class RequestHandlerBase<TRequest, TResponse> : RequestHandlerBase, IRequestHandler<TRequest, TResponse> {
	public sealed override object Execute(ProtocolOrchestrator orchestrator, object request) {
		Guard.ArgumentCast<TRequest>(request, out var requestT, nameof(request));
		var result = Execute(orchestrator, requestT);
		return result;
	}

	public abstract TResponse Execute(ProtocolOrchestrator orchestrator, TRequest request);

}
