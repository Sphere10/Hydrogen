namespace Sphere10.Framework.Communications {
    public interface IMessageGenerator<out TMessage> : IMessageGenerator {
		new TMessage Execute(ProtocolOrchestrator orchestrator);
	}
}
