using Sphere10.Framework;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Processor {
	/// <summary>
	/// IMPORTANT: This class deals exclusively with ALL inputs into the LocalQueue only.
	/// Inputs into and outputs out of the LocalQueue MUST be separated.
	/// </summary>
	public class LocalQueueInputProcessor : ILocalQueueInputProcessor {

		private readonly IHeliumQueue _localQueue;
		private readonly ILogger _logger;
		private readonly LocalQueueSettings _localQueueSettings;

		public LocalQueueInputProcessor(IHeliumQueue localQueue, ILogger logger) {
			_localQueue = localQueue;
			_logger = logger;

			_localQueueSettings = new LocalQueueSettings();

			if (_localQueue.RequiresLoad)
				_localQueue.Load();
		}

		public void AddMessageToLocalQueue(IMessage message) {
			_logger.Debug("Inside: LocalQueueInputProcessor.AddMessageToLocalQueue(_)");
			_logger.Debug("Adding a message to the LocalQueue.");

			using var txnScope = new FileTransactionScope(_localQueueSettings.TempDirPath);
			txnScope.BeginTransaction();
			txnScope.EnlistFile(_localQueue, false);

			using (_localQueue.EnterWriteScope()) {
					_localQueue.AddMessage(message);
			}

			txnScope.Commit();

			var totalMessagesInQueue = _localQueue.Count;
		}
	}
}