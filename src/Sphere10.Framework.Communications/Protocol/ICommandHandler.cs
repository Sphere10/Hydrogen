namespace Sphere10.Framework.Communications {

	public interface ICommandHandler<in TChannel, in TMessage> where TChannel : ProtocolChannel {
		void Execute(TChannel channel, TMessage command);
	}


}
