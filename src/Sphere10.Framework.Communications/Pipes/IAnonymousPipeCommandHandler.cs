namespace Sphere10.Framework.Communications {
	public interface IAnonymousPipeCommandHandler {
		void Execute(AnonymousPipeChannel channel, IAnonymousPipeMessage command);
	}
}
