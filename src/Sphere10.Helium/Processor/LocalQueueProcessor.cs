using System;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Processor {
	public class LocalQueueProcessor : ILocalQueueProcessor {

		private readonly IHeliumQueue _heliumQueue;
		private ILocalQueue _localQueue;
		private IProcessingQueue _processingQueue;

		public LocalQueueProcessor(BusConfiguration endpointConfiguration, IHeliumQueue heliumQueue) {
			_heliumQueue = heliumQueue;
			//_heliumQueue.FileName = endpointConfiguration.FileName;
		}

		ILocalQueue ILocalQueueProcessor.LocalQueue {
			get => _localQueue;
			set => _localQueue = value;
		}

		IProcessingQueue ILocalQueueProcessor.ProcessingQueue {
			get => _processingQueue;
			set => _processingQueue = value;
		}

		public void OnCommittedLocalQueue(object sender) {
			throw new NotImplementedException();
		}

		public void OnCommittedProcessingQueue(object sender) {
			throw new NotImplementedException();
		}

		public bool MoveMessageFromLocalToProcessing(IMessage message) {
			throw new NotImplementedException();
		}
	}
}
