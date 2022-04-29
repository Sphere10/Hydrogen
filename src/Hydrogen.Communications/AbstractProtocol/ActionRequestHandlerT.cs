using System;

namespace Sphere10.Framework.Communications {
    public class ActionRequestHandler<TRequest, TResponse> : RequestHandlerBase<TRequest, TResponse> {
		private readonly Func<ProtocolOrchestrator, TRequest, TResponse> _action;

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

}
