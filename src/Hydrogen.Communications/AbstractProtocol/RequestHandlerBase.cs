namespace Hydrogen.Communications {
    public abstract class RequestHandlerBase : IRequestHandler {
		public abstract object Execute(ProtocolOrchestrator orchestrator, object request);
    }
}
