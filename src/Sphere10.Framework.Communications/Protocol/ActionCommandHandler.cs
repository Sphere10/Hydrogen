using System;

namespace Sphere10.Framework.Communications {
	public class ActionCommandHandler<TChannel, TMessageBase, TMessage> : CommandHandlerBase<TChannel, TMessageBase>
		where TChannel : ProtocolChannel
		where TMessage : TMessageBase {
		private readonly Action<TChannel, TMessage> _action;

		public ActionCommandHandler(Action<TChannel, TMessage> action) {
			_action = action;
		}

		public override void Execute(TChannel channel, TMessageBase command) {
			_action.Invoke(channel, (TMessage)command);
		}
	}
}
