// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Communications;

public class ActionResponseHandler : ResponseHandlerBase {
	private readonly Action<ProtocolOrchestrator, object, object> _action;

	public ActionResponseHandler(Action<ProtocolOrchestrator, object, object> action) {
		Guard.ArgumentNotNull(action, nameof(action));
		_action = action;
	}

	public override Type RequestType => typeof(object);

	public override Type ResponseType => typeof(object);

	public override void Execute(ProtocolOrchestrator orchestrator, object request, object response) {
		Guard.ArgumentNotNull(orchestrator, nameof(orchestrator));
		Guard.ArgumentNotNull(request, nameof(request));
		Guard.ArgumentNotNull(request, nameof(response));
		_action.Invoke(orchestrator, request, response);
	}

}
