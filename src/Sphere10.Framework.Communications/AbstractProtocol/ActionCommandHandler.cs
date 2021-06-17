using System;

namespace Sphere10.Framework.Communications {

	public class ActionCommandHandler : CommandHandlerBase {
		private readonly Action<ProtocolChannel, object> _action;

		public ActionCommandHandler(Action<ProtocolChannel, object> action) {
			Guard.ArgumentNotNull(action, nameof(action));
			_action = action;
		}

		public override void Execute(ProtocolChannel channel, object command) {
			Guard.ArgumentNotNull(channel, nameof(channel));
			Guard.ArgumentNotNull(command, nameof(command));
			_action(channel, command);
		}
	}
}
