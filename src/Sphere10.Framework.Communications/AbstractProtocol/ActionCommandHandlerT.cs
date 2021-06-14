using System;

namespace Sphere10.Framework.Communications {

	public class ActionCommandHandler<TChannel, TMessage> : CommandHandlerBase<TChannel, TMessage>
		where TChannel : ProtocolChannel {
		private readonly Action<TChannel, TMessage> _action;

		public ActionCommandHandler(Action<TChannel, TMessage> action) {
			Guard.ArgumentNotNull(action, nameof(action));
			_action = action;
		}

		public override void Execute(TChannel channel, TMessage command) {
			Guard.ArgumentNotNull(channel, nameof(channel));
			Guard.ArgumentNotNull(command, nameof(command));
			_action.Invoke(channel, command);
		}
  
	}
}
