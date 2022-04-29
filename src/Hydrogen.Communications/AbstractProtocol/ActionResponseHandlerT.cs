using System;

namespace Sphere10.Framework.Communications {
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
