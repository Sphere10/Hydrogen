namespace Sphere10.Framework.Communications {
    public interface IResponseHandler {
		void Execute(ProtocolChannel channel, object request, object response);
    }
}
