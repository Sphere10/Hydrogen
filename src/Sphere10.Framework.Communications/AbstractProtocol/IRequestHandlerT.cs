namespace Sphere10.Framework.Communications {
    public interface IRequestHandler<in TChannel, in TRequest, TResponse> : IRequestHandler 
		where TChannel : ProtocolChannel {
		TResponse Execute(TChannel channel, TRequest request);
	}


}
