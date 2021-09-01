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
			Guard.Argument(message == null, nameof(message), "Message CANNOT be null here. Catastrophic failure!!!");

			_logger.Debug($"Inside:{nameof(LocalQueueInputProcessor)}_{MethodBase.GetCurrentMethod()}");
			_logger.Debug("Adding a single messageList to the LocalQueue.");
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

			Guard.Argument(messageList == null, nameof(messageList), "Message List CANNOT be null here. Catastrophic failure!!!");
			Guard.Argument(messageList != null && messageList.Count <= 0, nameof(messageList), "Input messageList count is <= 0. Lost input messages?");
			Guard.Argument(_settings.InputBufferSize <= 0, nameof(_settings.InputBufferSize), "Input messageList buffer size <= 0. Catastrophic failure!!!");

			_logger.Debug($"Adding a batch of {_settings.InputBufferSize} messages to the LocalQueue.");
			_logger.Debug($"Total messages in queue before add:{_localQueue.Count}");

			// ReSharper disable once PossibleNullReferenceException
			// ReSharper disable once PossibleLossOfFraction
			var loopCount = Math.Ceiling((decimal)(messageList.Count / _settings.InputBufferSize));

			for (var i = 0; i < loopCount; i++) {
				var messageBatch = messageList.Take(_settings.InputBufferSize);
				AddMessageBatchToLocalQueue(messageBatch);
			}
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