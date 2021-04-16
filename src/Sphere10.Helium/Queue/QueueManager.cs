using System;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Endpoint;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue {
	public class QueueManager : IQueueManager {

		private readonly IHeliumQueue _heliumQueue;

		public QueueManager(BusConfiguration endpointConfiguration, IHeliumQueue heliumQueue) {
			_heliumQueue = heliumQueue;
			//_heliumQueue.FileName = endpointConfiguration.FileName;
		}

		public void FirstIn(string destination, IMessage message) {
			_heliumQueue.AddMessageToQueue(message);
		}

		public void LastOut(IMessage message) {
			throw new NotImplementedException();
		}

		public void TakeThisMessageOffQueue(IMessage message) {
			throw new NotImplementedException();
		}

		public void PersistQueue() {
			throw new NotImplementedException();
		}


	}
}
