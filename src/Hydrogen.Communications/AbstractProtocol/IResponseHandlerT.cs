namespace Sphere10.Framework.Communications {
    public interface IResponseHandler<in TRequest, in TResponse> : IResponseHandler {
		void Execute(ProtocolOrchestrator orchestrator, TRequest request, TResponse response);
	}
}
