namespace Sphere10.Framework.Communications {
    public abstract class RequestHandlerBase : IRequestHandler {
		public abstract object Execute(ProtocolChannel channel, object request);
    }
}
