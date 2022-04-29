using System;

namespace Sphere10.Framework.Communications {

	public class ActionCommandHandler : CommandHandlerBase {
		private readonly Action<ProtocolOrchestrator, object> _action;

		public ActionCommandHandler(Action<ProtocolOrchestrator, object> action) {
			Guard.ArgumentNotNull(action, nameof(action));
			_action = action;
		}

		public override void Execute(ProtocolOrchestrator orchestrator, object command) {
			Guard.ArgumentNotNull(orchestrator, nameof(orchestrator));
			Guard.ArgumentNotNull(command, nameof(command));
			_action(orchestrator, command);
		}
	}
}
