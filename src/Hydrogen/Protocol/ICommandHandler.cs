namespace Hydrogen.Communications {
    public interface ICommandHandler {
		void Execute(ProtocolOrchestrator orchestrator, object command);
	}
}
