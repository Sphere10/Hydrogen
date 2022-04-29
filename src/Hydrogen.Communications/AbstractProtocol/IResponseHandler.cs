namespace Sphere10.Framework.Communications {
    public interface IResponseHandler {
		void Execute(ProtocolOrchestrator orchestrator, object request, object response);
    }
}
