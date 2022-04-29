using System;

namespace Sphere10.Framework.Communications {

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
}
