using System.Collections.Generic;
using Sphere10.Helium.Endpoint;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;
using Sphere10.Helium.Router;

namespace Sphere10.Helium.Route {

	/// <summary>
	/// The Router is single point of contact for the Node.
	/// ALL input and output to and from the Node goes through the Router.
	/// </summary>
	public interface IRouter {

		/// <summary>
		/// In code configuration for Router parameters.
		/// </summary>
		protected RouterConfigDto RouterConfigDto { get; set; }

		/// <summary>
		/// A list of all the messages already processed by the Router.
		/// Is NOT persisted: is empty on every Helium Service restart.
		/// </summary>
		protected IList<MessageProcessed> MessagesAlreadyProcessed { get; set; }

		/// <summary>
		/// This is the main method from which a message LEAVES the node going to another node via the Routers.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		protected bool OutputMessage(IMessage message);

		/// <summary>
		/// Contains a list of the destination addresses that the Router needs to send messages to.
		/// </summary>
		/// <returns></returns>
		protected IList<EndpointAddressListByTypeDto> GetEndpointAddresses();

		/// <summary>
		/// ALL single message input into the Node MUST only use this method.
		/// ALL other Routers or Helium Processes passes a single message as parameter to this method.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool InputMessage(IMessage message);

		/// <summary>
		/// ALL message Lists into the Node MUST only use this method.
		/// ALL other Routers or Helium Processes passes a message List as parameter to this method.
		/// The maxim amount of messages that can ever be in the is configurable.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool InputMessageList(IList<IMessage> message); //TODO Jake is there any evidence for Inputting multiple message in one method//

		/// <summary>
		/// 1) Message must have at least one property.
		/// 2) Check header fields have correct values.
		/// 3) Message's TimeToLive must be valid.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool CoreMessageValidation(IMessage message);

		/// <summary>
		/// 1) Check max items in list (configurable) is not exceeded.
		/// 2) All messages must have at least one property.
		/// 3) Check header fields have correct values.
		/// 4) Message's TimeToLive must be valid.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool CoreMessageValidation(IList<IMessage> message);

		/// <summary>
		/// Reliably put the message in LocalQueue and Commit.
		/// Happens when a message is received by the Input methods.
		/// </summary>
		/// <returns></returns>
		public bool PutMessageInLocalQueue();
		
		/// <summary>
		/// This event handler fires when a message is put in the RouterQueue by the Bus.
		/// </summary>
		/// <param name="sender"></param>
		public void OnMessageCommitted(object sender); //TODO Jake: how to deal with this sender object//

		/// <summary>
		/// Check if message has been processed previously.
		/// Use MessagesAlreadyProcessed List to check.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool WasMessageProcessedPreviously(IMessage message);
	}
}
