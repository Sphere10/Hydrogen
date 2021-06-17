namespace Sphere10.Framework.Communications {
    public abstract class RequestHandlerBase<TChannel, TRequest, TResponse> : RequestHandlerBase, IRequestHandler<TChannel, TRequest, TResponse>
		where TChannel : ProtocolChannel {
		public override object Execute(ProtocolChannel channel, object request) {
			Guard.ArgumentCast<TChannel>(channel, out var channelT, nameof(channel));
			Guard.ArgumentCast<TRequest>(request, out var requestT, nameof(request));
			var result = Execute(channelT, requestT);
			return result;
		}

		public abstract TResponse Execute(TChannel channel, TRequest request);

    }

}
