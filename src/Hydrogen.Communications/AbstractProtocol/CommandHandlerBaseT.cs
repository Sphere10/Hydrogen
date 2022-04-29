namespace Hydrogen.Communications {

    public abstract class CommandHandlerBase<TMessage> : CommandHandlerBase, ICommandHandler<TMessage> {

        public override void Execute(ProtocolOrchestrator orchestrator, object command) {
            Guard.ArgumentCast<TMessage>(command, out var commandT, nameof(command));
            Execute(orchestrator, commandT);
        }

        public abstract void Execute(ProtocolOrchestrator orchestrator, TMessage command);

    }
}
