using System.Collections.Generic;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Endpoint;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;
using Sphere10.Helium.Router;

namespace Sphere10.Helium.Route {
	/// <summary>
	///
	/// 1) The Router has an infinite loop that removes the last message from the queue (FIFO).
	/// 2) It will continue to do so until the queue is empty.
	/// 3) Even after the queue is empty it will keep on looping.
	///  
	/// </summary>

	public class Router : IRouter {
		private readonly IConfigureThisEndpoint _endpointConfiguration;
		private IHeliumQueue _routerQueue;
		private RouterConfigDto _routerConfigDto;
		private IList<MessageProcessed> _messagesAlreadyProcessed;

		public Router(IConfigureThisEndpoint endpointConfiguration) {
			_endpointConfiguration = endpointConfiguration;
		}


		IHeliumQueue IRouter.RouterQueue {
			get => _routerQueue;
			set => _routerQueue = value;
		}

		RouterConfigDto IRouter.RouterConfigDto {
			get => _routerConfigDto;
			set => _routerConfigDto = value;
		}

		bool IRouter.OutputMessage(IMessage message) {
			throw new System.NotImplementedException();
		}

		IList<EndpointAddressListByTypeDto> IRouter.GetEndpointAddresses() {
			throw new System.NotImplementedException();
		}

		IList<MessageProcessed> IRouter.MessagesAlreadyProcessed {
			get => _messagesAlreadyProcessed;
			set => _messagesAlreadyProcessed = value;
		}

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
	}
}
