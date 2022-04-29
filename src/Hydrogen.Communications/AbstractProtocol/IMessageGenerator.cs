namespace Sphere10.Framework.Communications {
    public interface IMessageGenerator {
		object Execute(ProtocolOrchestrator orchestrator);
	}
}
