namespace Sphere10.Framework.Communications {
    public interface ICommandHandler {
		void Execute(ProtocolOrchestrator orchestrator, object command);
	}
}
