// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Communications;

public class ActionRequestHandler : RequestHandlerBase {
	private readonly Func<ProtocolOrchestrator, object, object> _action;

	public override Type RequestType => typeof(object);

	public override Type ResponseType => typeof(object);

	public ActionRequestHandler(Func<ProtocolOrchestrator, object, object> action) {
		Guard.ArgumentNotNull(action, nameof(action));
		_action = action;
	}

	public override object Execute(ProtocolOrchestrator orchestrator, object request) {
		Guard.ArgumentNotNull(orchestrator, nameof(orchestrator));
		Guard.ArgumentNotNull(request, nameof(request));
		return _action(orchestrator, request);
	}
}
