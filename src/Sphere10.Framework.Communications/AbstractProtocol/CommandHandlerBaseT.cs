namespace Sphere10.Framework.Communications {

    public abstract class CommandHandlerBase<TChannel, TMessage> : CommandHandlerBase, ICommandHandler<TChannel, TMessage> 
		where TChannel : ProtocolChannel {

        public override void Execute(ProtocolChannel channel, object command) {
            Guard.ArgumentCast<TChannel>(channel, out var channelT, nameof(channel));
            Guard.ArgumentCast<TMessage>(command, out var commandT, nameof(command));
            Execute(channelT, commandT);
        }

        public abstract void Execute(TChannel channel, TMessage command);

    }
}
