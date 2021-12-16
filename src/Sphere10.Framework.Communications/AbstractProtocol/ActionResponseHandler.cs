using System;

namespace Sphere10.Framework.Communications {
    public class ActionResponseHandler : ResponseHandlerBase {
		private readonly Action<ProtocolOrchestrator, object, object> _action;

		public ActionResponseHandler(Action<ProtocolOrchestrator, object, object> action) {
			Guard.ArgumentNotNull(action, nameof(action));
			_action = action;
		}

		public override void Execute(ProtocolOrchestrator orchestrator, object request, object response) {
			Guard.ArgumentNotNull(orchestrator, nameof(orchestrator));
			Guard.ArgumentNotNull(request, nameof(request));
			Guard.ArgumentNotNull(request, nameof(response));
			_action.Invoke(orchestrator, request, response);
		}
		
	}

}
