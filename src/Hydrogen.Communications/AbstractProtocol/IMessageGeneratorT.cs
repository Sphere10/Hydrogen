namespace Hydrogen.Communications {
    public interface IMessageGenerator<out TMessage> : IMessageGenerator {
		new TMessage Execute(ProtocolOrchestrator orchestrator);
	}
}
