using System;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Processor {
	public class LocalQueueProcessor : ILocalQueueProcessor {

		private readonly IHeliumQueue _heliumQueue;

		public LocalQueueProcessor(BusConfiguration endpointConfiguration, IHeliumQueue heliumQueue) {
			_heliumQueue = heliumQueue;
			//_heliumQueue.FileName = endpointConfiguration.FileName;
		}

		public void FirstIn(string destination, IMessage message) {
			_heliumQueue.AddMessage(message);
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
