// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
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
