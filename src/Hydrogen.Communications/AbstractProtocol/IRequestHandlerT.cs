namespace Hydrogen.Communications {
    public interface IRequestHandler<in TRequest, out TResponse> : IRequestHandler {
		TResponse Execute(ProtocolOrchestrator orchestrator, TRequest request);
	}


}
