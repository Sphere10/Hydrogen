using System.Collections.Generic;
using Sphere10.Framework;
using Sphere10.Helium.Endpoint;
using Sphere10.Helium.Message;
using Sphere10.Helium.Processor;

namespace Sphere10.Helium.Router {
	public class Router : IRouter {
		private readonly ILocalQueueInput _localQueueInput;

		public Router(ILocalQueueInput localQueueInput) {
			_localQueueInput = localQueueInput;
		}

		public ILogger Logger { get; set; }

		bool IRouter.OutputMessage(IMessage message) {

			throw new System.NotImplementedException();
		}

		IList<EndpointAddressListByTypeDto> IRouter.GetEndpointAddresses() {
			
			throw new System.NotImplementedException();
		}

		IList<MessageProcessed> IRouter.MessagesAlreadyProcessed { get; set; }

		public void InputMessage(IMessage message) {
			Logger.Debug("Inbound message received: Router.InputMessage(_)");
			Logger.Debug($"Message Name = {message.GetType().Name}");
			Logger.Debug($"Message Assembly = {message.GetType().Assembly}");

			_localQueueInput.AddMessageToLocalQueue(message);
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
