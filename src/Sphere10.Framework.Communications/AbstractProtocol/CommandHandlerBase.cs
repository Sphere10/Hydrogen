namespace Sphere10.Framework.Communications {

    public abstract class CommandHandlerBase : ICommandHandler {
        public abstract void Execute(ProtocolChannel channel, object command);
    }
}
