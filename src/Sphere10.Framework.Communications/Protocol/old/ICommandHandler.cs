namespace Sphere10.Framework.Communications.Protocol {
	public interface ICommandHandler<in TEndpoint, in TMessage> {
		void Execute(TMessage command, TEndpoint caller, ProtocolChannelInitiator channelInitiator);
	}
}
