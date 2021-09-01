using System.Collections.Generic;
using System.Reflection;
using Sphere10.Framework;
using Sphere10.Helium.Endpoint;
using Sphere10.Helium.Message;
using Sphere10.Helium.Processor;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Router {
	public class Router : IRouter {
		private readonly ILocalQueueInputProcessor _localQueueInput;
		private readonly LocalQueueSettings _settings;

		public Router(ILocalQueueInputProcessor localQueueInput) {
			_localQueueInput = localQueueInput;
			_settings = new LocalQueueSettings();
		}

		public ILogger Logger { get; set; }

		bool IRouter.OutputMessage(IMessage message) {

			throw new System.NotImplementedException();
		}

		IList<EndpointAddressListByTypeDto> IRouter.GetEndpointAddresses() {

			throw new System.NotImplementedException();
		}

		IList<MessageProcessed> IRouter.MessagesAlreadyProcessed { get; set; }

		public bool InputMessage(IMessage message) {
			if (message == null) return false;

			Logger.Debug($"Inside:{nameof(Router)}_{MethodBase.GetCurrentMethod()}");
			Logger.Debug($"Message Name = {message.GetType().Name}");
			Logger.Debug($"Message Assembly = {message.GetType().Assembly}");

			_localQueueInput.AddMessageToLocalQueue(message);

			return true; /*Needed for StandAloneMode*/
		}

		public bool InputMessageList(IList<IMessage> messageList) {
			if (messageList == null || messageList.Count <= 0) return false;

			Logger.Debug($"Inside:{nameof(Router)}_{MethodBase.GetCurrentMethod()}");
			Logger.Debug($"Message count={messageList.Count}");

			Guard.Argument(messageList.Count >= _settings.InputBufferLimit, nameof(_settings.InputBufferLimit), "Seriously sending 10,000 plus messages in one hit? Consider smaller batches!");

			_localQueueInput.AddMessageListToLocalQueue(messageList);

			return true; /*Needed for StandAloneMode*/
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
