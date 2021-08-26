using System.Collections.Generic;
using Sphere10.Framework;
using Sphere10.Helium.Framework;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Processor {
	/// <summary>
	/// IMPORTANT: This class processes all messages in the LocalQueue. It deals exclusively with ALL Reading/Deleting/Re-Inserting of the LocalQueue.
	/// </summary>
	public class LocalQueueOutputProcessor : ILocalQueueOutputProcessor {
		private readonly IInstantiateHandler _instantiateHandler;
		private readonly IHeliumQueue _localQueue;
		private readonly ILocalQueueInputProcessor _localQueueInput;
		private readonly ILogger _logger;
		public IList<IMessage> _currentMessageList;

		public LocalQueueOutputProcessor(IInstantiateHandler instantiateHandler, 
		                           IHeliumQueue localQueue, 
		                           ILocalQueueInputProcessor localQueueInput,
		                           ILogger logger) {

			_currentMessageList = new List<IMessage>();
			_instantiateHandler = instantiateHandler;
			_localQueue = localQueue;
			_localQueueInput = localQueueInput;
			_logger = logger;

			if (_localQueue.RequiresLoad)
				_localQueue.Load();

			_localQueue.Committed += OnCommittedLocalQueue;
		}
		
		public void OnCommittedLocalQueue(object sender) {
			_logger.Debug("Inside: LocalQueueOutputProcessor.OnCommittedLocalQueue(_)");
			_logger.Debug("LocalQueue's Committed event fired!");

			ProcessAllMessagesSynchronously();
		}
		
		public void ProcessAllMessagesSynchronously() {
			_logger.Debug("Inside: LocalQueueOutputProcessor.ProcessAllMessagesSynchronously()");
			_logger.Debug("Processing ALL messages from LocalQueue.");

			var messageCount = _localQueue.Count;

			for (var i = 0; i < messageCount; i++) {
				var message =_localQueue.RemoveMessage();
				_currentMessageList.Add(message);
			}

			ExecuteHandler(_currentMessageList);
		}

		public void ExecuteHandler(IList<IMessage> messageList) {
			foreach (var message in messageList) {
				
			}
		}
	}
}