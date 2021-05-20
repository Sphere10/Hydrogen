using System.Collections.Generic;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Route {

	/// <summary>
	/// The Router is single point of contact for the Node.
	/// ALL input and output to and from the Node goes through the Router.
	/// </summary>
	public interface IRouter {

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
		public bool InputMessageList(IList<IMessage> message);

		public void ReadLastMessageFromQueue();

		public bool CoreMessageValidation(IMessage message);

		public bool CoreMessageValidation(IList<IMessage> message);
	}
}
