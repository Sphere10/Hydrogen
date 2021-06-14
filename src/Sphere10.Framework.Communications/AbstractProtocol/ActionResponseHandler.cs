using System;

namespace Sphere10.Framework.Communications {
    public class ActionResponseHandler : ResponseHandlerBase {
		private readonly Action<ProtocolChannel, object, object> _handler;

		public ActionResponseHandler(Action<ProtocolChannel, object, object> handler) {
			_handler = handler;
		}

		public override void Execute(ProtocolChannel channel, object request, object response) 
			=>_handler.Invoke(channel, request, response);
		
	}

}
