namespace Sphere10.Framework.Protocol {
	public interface IRequestHandler<in TEndpoint, TMessage> {
		TMessage Execute(TMessage command, TEndpoint caller, ProtocolChannelInitiator channelInitiator);
	}
}
