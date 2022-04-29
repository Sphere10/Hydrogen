namespace Sphere10.Framework.Communications {

    public interface ICommandHandler<in TMessage> : ICommandHandler {
		void Execute(ProtocolOrchestrator orchestrator, TMessage command);
	}
}
