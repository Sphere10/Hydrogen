using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
		private readonly LocalQueueSettings _settings;

		// ReSharper disable once NotAccessedField.Local
		private readonly ILocalQueueInputProcessor _localQueueInput;
		private readonly ILogger _logger;

		public LocalQueueOutputProcessor(IInstantiateHandler instantiateHandler,
								   IHeliumQueue localQueue,
								   ILocalQueueInputProcessor localQueueInput,
								   ILogger logger) {

			_settings = new LocalQueueSettings();
			_instantiateHandler = instantiateHandler;
			_localQueue = localQueue;
			_localQueueInput = localQueueInput;
			_logger = logger;

			if (_localQueue.RequiresLoad)
				_localQueue.Load();

			_localQueue.Committed += OnCommittedLocalQueue;
		}

		public void OnCommittedLocalQueue(object sender) {
			_logger.Debug($"Inside:{nameof(LocalQueueOutputProcessor)}_{MethodBase.GetCurrentMethod()}");

			ProcessAllMessagesInQueue();
		}

		public void ProcessAllMessagesInQueue() {
			_logger.Debug($"Inside:{nameof(LocalQueueOutputProcessor)}_{MethodBase.GetCurrentMethod()}");

			var currentMessageList = new List<IMessage>();

			var messageCount = _localQueue.Count;
			_logger.Debug($"Total messages in queue before Remove:{messageCount}");

			for (var i = 0; i < messageCount; i++) {
				var message = _localQueue.RemoveMessage();
				currentMessageList.Add(message);
			}

			ExtractHandler(currentMessageList);
		}

		private void ExtractHandler(List<IMessage> currentMessageList) {
			_logger.Debug($"Inside:{nameof(LocalQueueOutputProcessor)}_{MethodBase.GetCurrentMethod()}");
			_logger.Debug($"Total of {_instantiateHandler.PluginAssemblyHandlerList.Count} handlers to check.");

			if (currentMessageList == null) throw new ArgumentNullException(nameof(currentMessageList));
			if (_settings.AmountOfProcessingThreads <= 0) throw new ArgumentNullException(nameof(_settings.AmountOfProcessingThreads));
			var loopCount = Math.Ceiling(d: currentMessageList.Count / (decimal)_settings.AmountOfProcessingThreads);

			for (var i = 0; i < loopCount; i++) {
				var taskMessageBatch = currentMessageList.Take((int)_settings.AmountOfProcessingThreads);
				var enumTaskMessageBatch = taskMessageBatch.ToList();
				currentMessageList = currentMessageList.Except(enumTaskMessageBatch).ToList();
				
				var taskArray = new Task[enumTaskMessageBatch.Count];
				
				var j = 0;
				foreach (var taskMessage in enumTaskMessageBatch) {
					taskArray[j] = Task.Factory.StartNew(() => InitTaskForHandlerList(taskMessage));
					j++;
				}
				
				Task.WaitAll(taskArray);
			}
		}

		private void InitTaskForHandlerList(IMessage message) {
				_logger.Debug($"Finding Handlers for message={message.GetType()}");

				var handlerTypeList = _instantiateHandler.PluginAssemblyHandlerList;
				var handlerList = handlerTypeList
					.Where(x => x.Message.FullName != null && x.Message.FullName.Equals(message.GetType().FullName))
					.ToList();

				if (handlerList.Count <= 0) return;

				InvokeHandler(message, handlerList);
		}

		private void InvokeHandler(IMessage message, IReadOnlyCollection<PluginAssemblyHandlerDto> handlerTypeList) {
			_logger.Debug($"Inside:{nameof(LocalQueueOutputProcessor)}_{MethodBase.GetCurrentMethod()}");
			_logger.Debug($"Total of {handlerTypeList.Count} Handlers found.");

			foreach (var handlerType in handlerTypeList) {
				_logger.Debug($"HandlerInterface to run={handlerType.HandlerInterface}");
				_logger.Debug($"Handler name={handlerType.HandlerClass.Name}, full-name={handlerType.HandlerClass.FullName}.");

				var handler = Activator.CreateInstance(handlerType.HandlerClass, null);
				var parameterTypes = new[] { handlerType.Message };
				var handleMethod = handlerType.HandlerClass.GetMethod("Handle", parameterTypes);
				var messageFromTypeInstance = Activator.CreateInstance(handlerType.Message);

				AssignPropertyValues(message, messageFromTypeInstance);

				var parameters = new[] { messageFromTypeInstance };

				_logger.Debug("Invoking Handler now...");
				handleMethod?.Invoke(handler, parameters);
				_logger.Debug("Hooray! Handler invoked successfully!");
			}
		}

		private static void AssignPropertyValues(IMessage message, object messageFromTypeInstance) {
			foreach (var propertyInfo in message.GetType().GetProperties()) {
				var property = messageFromTypeInstance?.GetType().GetProperty(propertyInfo.Name, propertyInfo.PropertyType);
				var propertyValue = propertyInfo.GetValue(message);
				property?.SetValue(messageFromTypeInstance, propertyValue, null);
			}
		}
	}
}