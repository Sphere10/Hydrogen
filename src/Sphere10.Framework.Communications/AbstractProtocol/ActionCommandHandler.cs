using System;

namespace Sphere10.Framework.Communications {

	public class ActionCommandHandler : CommandHandlerBase {
		private readonly Action<ProtocolChannel, object> _action;

		public ActionCommandHandler(Action<ProtocolChannel, object> action) {
			_action = action;
		}

		public override void Execute(ProtocolChannel channel, object command) {
			_action.Invoke(channel, command);
		}
	}
}
