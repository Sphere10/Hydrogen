namespace Sphere10.Framework.Communications {
    public abstract class RequestHandlerBase<TRequest, TResponse> : RequestHandlerBase, IRequestHandler<TRequest, TResponse> {
		public sealed override object Execute(ProtocolOrchestrator orchestrator, object request) {
			Guard.ArgumentCast<TRequest>(request, out var requestT, nameof(request));
			var result = Execute(orchestrator, requestT);
			return result;
		}

		public abstract TResponse Execute(ProtocolOrchestrator orchestrator, TRequest request);

    }

}
