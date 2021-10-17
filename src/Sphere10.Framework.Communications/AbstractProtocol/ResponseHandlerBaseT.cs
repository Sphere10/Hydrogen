namespace Sphere10.Framework.Communications {
    public abstract class ResponseHandlerBase<TRequest, TResponse> : ResponseHandlerBase, IResponseHandler<TRequest, TResponse> {
		public sealed override void Execute(ProtocolOrchestrator orchestrator, object request, object response) {
			Guard.ArgumentCast<TRequest>(request, out var requestT, nameof(request));
			Guard.ArgumentCast<TResponse>(response, out var responseT, nameof(response));
			Execute(orchestrator, requestT, responseT);
		}

		public abstract void Execute(ProtocolOrchestrator orchestrator, TRequest request, TResponse response);

	}
}
