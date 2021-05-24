namespace Sphere10.Framework.Communications {
	
	public abstract class AnonymousPipeHandler<TMessage> : AnonymousPipeHandler where TMessage : IAnonymousPipeMessage {
		public override void Execute(AnonymousPipeChannel channel, IAnonymousPipeMessage command) {
			Execute(channel, (TMessage)command);
		}
		public abstract void Execute(AnonymousPipeChannel channel, TMessage command);
	}


	public abstract class AnonymousPipeHandler : IAnonymousPipeCommandHandler {
		public abstract void Execute(AnonymousPipeChannel channel, IAnonymousPipeMessage command);
	}
}
