//using System;

//namespace Sphere10.Framework.Communications {
//	public class ActionAnonymousPipeCommandHandler<TMessage> : AnonymousPipeHandler<TMessage> where TMessage : IAnonymousPipeMessage {
//		private readonly Action<AnonymousPipe, TMessage> _action;

//		public ActionAnonymousPipeCommandHandler(Action<AnonymousPipe, TMessage> action) {
//			_action = action;
//		}

//		public override void Execute(AnonymousPipe channel, TMessage command) {
//			_action.Invoke(channel, command);
//		}
//	}
//}
