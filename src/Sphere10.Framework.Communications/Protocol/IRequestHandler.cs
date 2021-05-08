namespace Sphere10.Framework.Communications.Protocol {
	public interface IRequestHandler<in TEndpoint, TMessage> {
		TMessage Execute(TMessage command, TEndpoint caller, ProtocolChannelInitiator channelInitiator);
	}
}
