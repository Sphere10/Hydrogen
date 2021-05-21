using System.Collections.Generic;
using Sphere10.Helium.Endpoint;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;
using Sphere10.Helium.Router;

namespace Sphere10.Helium.Route {

	public class Router : IRouter {

		private readonly IConfigureThisEndpoint _endpointConfiguration;
		private readonly IHeliumQueue _routerQueue;

		public Router(IConfigureThisEndpoint endpointConfiguration, IHeliumQueue routerQueue) {
			_endpointConfiguration = endpointConfiguration;
			_routerQueue = routerQueue;
		}

		IRouterQueue IRouter.RouterQueue { get; set; }

		RouterConfigDto IRouter.RouterConfigDto { get; set; }

		bool IRouter.OutputMessage(IMessage message) {
			var x = _routerQueue.Count; //TODO: Jake remove this. Done to get rid of Not Used warning.//

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
