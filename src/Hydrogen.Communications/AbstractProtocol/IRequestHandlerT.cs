namespace Sphere10.Framework.Communications {
    public interface IRequestHandler<in TRequest, out TResponse> : IRequestHandler {
		TResponse Execute(ProtocolOrchestrator orchestrator, TRequest request);
	}


}
