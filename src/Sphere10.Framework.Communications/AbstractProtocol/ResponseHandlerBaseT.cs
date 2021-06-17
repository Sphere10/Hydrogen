namespace Sphere10.Framework.Communications {
    public abstract class ResponseHandlerBase<TChannel, TRequest, TResponse> : ResponseHandlerBase, IResponseHandler<TChannel, TRequest, TResponse>
		where TChannel : ProtocolChannel {
		public override void Execute(ProtocolChannel channel, object request, object response) {
			Guard.ArgumentCast<TChannel>(channel, out var channelT, nameof(channel));
			Guard.ArgumentCast<TRequest>(request, out var requestT, nameof(request));
			Guard.ArgumentCast<TResponse>(response, out var responseT, nameof(response));
			Execute(channelT, requestT, responseT);
		}

		public abstract void Execute(TChannel channel, TRequest request, TResponse response);

	}
}
