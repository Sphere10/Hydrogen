namespace Sphere10.Framework.Communications {
	public interface IAnonymousPipeRequestHandler {
		IAnonymousPipeMessage Execute(AnonymousPipeChannel channel, IAnonymousPipeMessage command);
	}
}
