using System;

namespace Sphere10.Framework.Communications {
	public class ActionAnonymousPipeCommandHandler<TMessage> : AnonymousPipeHandler<TMessage> where TMessage : IAnonymousPipeMessage {
		private readonly Action<AnonymousPipeChannel, TMessage> _action;

		public ActionAnonymousPipeCommandHandler(Action<AnonymousPipeChannel, TMessage> action) {
			_action = action;
		}

		public override void Execute(AnonymousPipeChannel channel, TMessage command) {
			_action.Invoke(channel, command);
		}
	}
}
