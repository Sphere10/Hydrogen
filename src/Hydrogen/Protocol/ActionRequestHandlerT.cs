// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Communications;

public class ActionRequestHandler<TRequest, TResponse> : RequestHandlerBase<TRequest, TResponse> {
	private readonly Func<ProtocolOrchestrator, TRequest, TResponse> _action;

	public override Type RequestType => typeof(TRequest);

	public override Type ResponseType => typeof(TResponse);

	public ActionRequestHandler(Func<ProtocolOrchestrator, TRequest, TResponse> action) {
		Guard.ArgumentNotNull(action, nameof(action));
		_action = action;
	}

	public override TResponse Execute(ProtocolOrchestrator orchestrator, TRequest request) {
		Guard.ArgumentNotNull(orchestrator, nameof(orchestrator));
		Guard.ArgumentNotNull(request, nameof(request));
		return _action(orchestrator, request);
	}

}
