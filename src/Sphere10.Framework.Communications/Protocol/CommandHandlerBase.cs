namespace Sphere10.Framework.Communications {

	public abstract class CommandHandlerBase<TChannel, TMessage> : ICommandHandler<TChannel, TMessage> 
		where TChannel : ProtocolChannel{
		public abstract void Execute(TChannel channel, TMessage command);
	}
}
