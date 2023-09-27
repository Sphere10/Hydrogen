// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Communications;

public class ActionCommandHandler<TMessage> : CommandHandlerBase<TMessage> {
	private readonly Action<ProtocolOrchestrator, TMessage> _action;

	public ActionCommandHandler(Action<ProtocolOrchestrator, TMessage> action) {
		Guard.ArgumentNotNull(action, nameof(action));
		_action = action;
	}

	public override void Execute(ProtocolOrchestrator orchestrator, TMessage command) {
		Guard.ArgumentNotNull(orchestrator, nameof(orchestrator));
		Guard.ArgumentNotNull(command, nameof(command));
		_action.Invoke(orchestrator, command);
	}

}
