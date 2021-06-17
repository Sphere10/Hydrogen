namespace Sphere10.Framework.Communications {

    public interface ICommandHandler<in TChannel, in TMessage> : ICommandHandler where TChannel : ProtocolChannel {
		void Execute(TChannel channel, TMessage command);
	}
}
