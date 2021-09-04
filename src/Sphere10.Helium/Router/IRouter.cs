using System.Collections.Generic;
using Sphere10.Framework;
using Sphere10.Helium.HeliumNode;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Router {

	/// <summary>
	/// The Router is a single point of contact for the Node.
	/// ALL input and output to and from the Node goes through the Router.
	/// </summary>
	public interface IRouter {
		public ILogger Logger { get; set; }

		protected IList<MessageProcessed> MessagesAlreadyProcessed { get; set; }
		protected bool OutputMessage(IMessage message);
		protected IList<EndpointAddressListByTypeDto> GetEndpointAddresses();

		public bool InputMessage(IMessage message);
		public bool InputMessageList(IList<IMessage> messageList);
		public bool CoreMessageValidation(IMessage message);
		public bool CoreMessageValidation(IList<IMessage> message);
		public bool PutMessageInLocalQueue();
		public void OnMessageCommitted(object sender); //TODO Jake: how to deal with this sender object//
		public bool WasMessageProcessedPreviously(IMessage message);
	}
}