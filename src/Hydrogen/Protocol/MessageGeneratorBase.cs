namespace Hydrogen.Communications {
    public abstract class MessageGeneratorBase : IMessageGenerator {
		public abstract object Execute(ProtocolOrchestrator orchestrator);
    }
}
