// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Communications {
    public class ActionResponseHandler<TRequest, TResponse> : ResponseHandlerBase<TRequest, TResponse> { 
		private readonly Action<ProtocolOrchestrator, TRequest, TResponse> _action;

		public ActionResponseHandler(Action<ProtocolOrchestrator, TRequest, TResponse> action) {
			Guard.ArgumentNotNull(action, nameof(action));
			_action = action;
		}

		public override void Execute(ProtocolOrchestrator orchestrator, TRequest request, TResponse response) {
			Guard.ArgumentNotNull(orchestrator, nameof(orchestrator));
			Guard.ArgumentNotNull(request, nameof(request));
			Guard.ArgumentNotNull(request, nameof(response));
			_action(orchestrator, request, response);
		}

	}

}
