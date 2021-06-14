namespace Sphere10.Framework.Communications {
    public interface ICommandHandler {
		void Execute(ProtocolChannel channel, object command);
	}
}
