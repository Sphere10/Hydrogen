using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
		private readonly LocalQueueSettings _settings;

		public LocalQueueInputProcessor(IHeliumQueue localQueue, ILogger logger) {
			_localQueue = localQueue;
			_logger = logger;
			_settings = new LocalQueueSettings();

			if (_localQueue.RequiresLoad) _localQueue.Load();
		}

		public void AddMessageToLocalQueue(IMessage message) {
			Guard.Argument(message != null, nameof(message), "Message == null. Unexpected. Catastrophic failure!");

			_logger.Debug($"Inside:{nameof(LocalQueueInputProcessor)}_{MethodBase.GetCurrentMethod()}");
			_logger.Debug("Adding a single message to the LocalQueue.");
			_logger.Debug($"Total messages in queue before add:{_localQueue.Count}");

			using var txnScope = new FileTransactionScope(_settings.TempDirPath);
			txnScope.BeginTransaction();
			txnScope.EnlistFile(_localQueue, false);

			using (_localQueue.EnterWriteScope()) {
				_localQueue.AddMessage(message);
			}

			txnScope.Commit();
		}

		public void AddMessageListToLocalQueue(IList<IMessage> messageList) {
			_logger.Debug($"Inside:{nameof(LocalQueueInputProcessor)}_{MethodBase.GetCurrentMethod()}");

			Guard.Argument(messageList != null, nameof(messageList), "MessageList == null. UNEXPECTED. Catastrophic failure!!!");
			Guard.Argument(messageList != null && messageList.Count > 0, nameof(messageList), "messageList count <= 0. Lost input messages! BAD.");
			Guard.Argument(_settings.InputMessageBatchSize > 0, nameof(_settings.InputMessageBatchSize), "messageList buffer <= 0. Catastrophic failure!!!");

			_logger.Debug($"Adding a batch of {_settings.InputMessageBatchSize} messages to the LocalQueue.");
			_logger.Debug($"Total messages in queue before add:{_localQueue.Count}");

			if (messageList == null) throw new ArgumentNullException(nameof(messageList));
			if (_settings.InputMessageBatchSize <= 0) throw new ArgumentNullException(nameof(_settings.InputMessageBatchSize));
			var loopCount = Math.Ceiling(d: messageList.Count / (decimal)_settings.InputMessageBatchSize);

			for (var i = 0; i < loopCount; i++) {
				var messageBatch = messageList.Take(_settings.InputMessageBatchSize);
				var enumMessageBatch = messageBatch.ToList();

				messageList = messageList.Except(enumMessageBatch).ToList();

				AddMessageBatchToLocalQueue(enumMessageBatch);
			}
		}

		public void FlushLocalQueue() {
			_localQueue.Clear();
		}

		private void AddMessageBatchToLocalQueue(IEnumerable<IMessage> messageBatch) {
			using var txnScope = new FileTransactionScope(_settings.TempDirPath);
			txnScope.BeginTransaction();
			txnScope.EnlistFile(_localQueue, false);

			using (_localQueue.EnterWriteScope()) {
				_localQueue.AddRange(messageBatch);
			}

			txnScope.Commit();
		}
	}
}