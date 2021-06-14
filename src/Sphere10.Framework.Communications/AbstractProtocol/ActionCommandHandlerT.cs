using System;

namespace Sphere10.Framework.Communications {

	public class ActionCommandHandler<TChannel, TMessage> : CommandHandlerBase<TChannel, TMessage>
		where TChannel : ProtocolChannel {
		private readonly Action<TChannel, TMessage> _action;

		public ActionCommandHandler(Action<TChannel, TMessage> action) {
			_action = action;
		}

		public override void Execute(TChannel channel, TMessage command) {
			Guard.ArgumentCast<TChannel>(channel, out var channelT, nameof(channel));
			Guard.ArgumentCast<TMessage>(command, out var commandT, nameof(command));
			Execute(channelT, commandT);
		}
  
	}
}
