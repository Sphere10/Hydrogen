// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Communications;

public class ActionMessageGeneratorT<TMessage> : MessageGeneratorBase<TMessage> {
	private readonly Func<ProtocolOrchestrator, TMessage> _initiator;

	public ActionMessageGeneratorT(Func<ProtocolOrchestrator, TMessage> initiator) {
		Guard.ArgumentNotNull(initiator, nameof(initiator));
		_initiator = initiator;
	}

	protected override TMessage ExecuteInternal(ProtocolOrchestrator orchestrator)
		=> _initiator.Invoke(orchestrator);
}
