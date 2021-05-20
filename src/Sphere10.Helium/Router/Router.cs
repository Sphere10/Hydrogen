using System.Collections.Generic;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Endpoint;
using Sphere10.Helium.Message;

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

		public Router(IConfigureThisEndpoint endpointConfiguration) {
			_endpointConfiguration = endpointConfiguration;
		}


		public bool InputMessage(IMessage message) {
			throw new System.NotImplementedException();
		}

		public bool InputMessageList(IList<IMessage> message) {
			throw new System.NotImplementedException();
		}

		public void ReadLastMessageFromQueue() {
			throw new System.NotImplementedException();
		}

		public bool CoreMessageValidation(IMessage message) {
			throw new System.NotImplementedException();
		}

		public bool CoreMessageValidation(IList<IMessage> message) {
			throw new System.NotImplementedException();
		}
	}
}
