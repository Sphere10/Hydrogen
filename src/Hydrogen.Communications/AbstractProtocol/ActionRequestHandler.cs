using System;

namespace Sphere10.Framework.Communications {
    public class ActionRequestHandler : RequestHandlerBase {
		private readonly Func<ProtocolOrchestrator, object, object> _action;

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

}
