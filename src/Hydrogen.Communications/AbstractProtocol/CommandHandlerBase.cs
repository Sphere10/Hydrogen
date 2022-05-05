namespace Hydrogen.Communications {

    public abstract class CommandHandlerBase : ICommandHandler {
        public abstract void Execute(ProtocolOrchestrator orchestrator, object command);
    }
}
