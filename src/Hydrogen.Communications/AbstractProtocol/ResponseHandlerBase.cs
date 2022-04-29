namespace Hydrogen.Communications {
    public abstract class ResponseHandlerBase : IResponseHandler {
        public abstract void Execute(ProtocolOrchestrator orchestrator, object request, object response);
    }
}
