namespace Sphere10.Framework.Communications {
    public interface IResponseHandler<in TChannel, in TRequest, in TResponse> : IResponseHandler
		where TChannel : ProtocolChannel {
		void Execute(TChannel channel, TRequest request, TResponse response);
	}
}
