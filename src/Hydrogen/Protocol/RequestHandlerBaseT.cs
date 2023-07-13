// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
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
