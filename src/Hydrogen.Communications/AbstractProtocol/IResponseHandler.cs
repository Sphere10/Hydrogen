namespace Hydrogen.Communications {
    public interface IResponseHandler {
		void Execute(ProtocolOrchestrator orchestrator, object request, object response);
    }
}
