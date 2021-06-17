using System.Collections.Generic;
using Sphere10.Helium.Endpoint;
using Sphere10.Helium.Message;
using Sphere10.Helium.Retry;

namespace Sphere10.Helium.Router {

	public class Router : IRouter {
		private readonly IRetryManager _retryManager;
		//private readonly IHeliumQueue _routerQueue;
		//private readonly IHeliumQueue _localQueue;

		//public Router(IHeliumQueue routerQueue, IHeliumQueue localQueue) {
		//	_routerQueue = routerQueue;
		//	_localQueue = localQueue;
		//}

		public Router(IRetryManager retryManager) {
			_retryManager = retryManager;

		}

		RouterConfigDto IRouter.RouterConfigDto { get; set; }

		bool IRouter.OutputMessage(IMessage message) {

			throw new System.NotImplementedException();
		}

		IList<EndpointAddressListByTypeDto> IRouter.GetEndpointAddresses() {
			throw new System.NotImplementedException();
		}

		IList<MessageProcessed> IRouter.MessagesAlreadyProcessed { get; set; }

		public bool InputMessage(IMessage message) {
			throw new System.NotImplementedException();
		}

		public bool InputMessageList(IList<IMessage> message) {
			throw new System.NotImplementedException();
		}

		public bool CoreMessageValidation(IMessage message) {
			throw new System.NotImplementedException();
		}

		public bool CoreMessageValidation(IList<IMessage> message) {
			throw new System.NotImplementedException();
		}

		public bool PutMessageInLocalQueue() {
			throw new System.NotImplementedException();
		}

		public void OnMessageCommitted(object sender) {
			throw new System.NotImplementedException();
		}

		public bool WasMessageProcessedPreviously(IMessage message) {
			throw new System.NotImplementedException();
		}
	}
}
