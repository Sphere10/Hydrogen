namespace Sphere10.Framework.Communications {
    public abstract class ResponseHandlerBase : IResponseHandler {
        public abstract void Execute(ProtocolChannel channel, object request, object response);
    }
}
