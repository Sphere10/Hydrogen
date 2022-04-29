namespace Hydrogen.Communications {
    public interface IMessageGenerator {
		object Execute(ProtocolOrchestrator orchestrator);
	}
}
