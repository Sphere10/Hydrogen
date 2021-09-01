using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

		// ReSharper disable once NotAccessedField.Local
		private readonly ILocalQueueInputProcessor _localQueueInput;
		private readonly ILogger _logger;
		public IList<IMessage> CurrentMessageList;

		public LocalQueueOutputProcessor(IInstantiateHandler instantiateHandler,
								   IHeliumQueue localQueue,
								   ILocalQueueInputProcessor localQueueInput,
								   ILogger logger) {

			CurrentMessageList = new List<IMessage>();
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

			ProcessAllMessagesSynchronously();
		}

		public void ProcessAllMessagesSynchronously() {
			_logger.Debug($"Inside:{nameof(LocalQueueOutputProcessor)}_{MethodBase.GetCurrentMethod()}");

			CurrentMessageList.Clear();

			var messageCount = _localQueue.Count;
			_logger.Debug($"Total messages in queue before Remove:{messageCount}");

			for (var i = 0; i < messageCount; i++) {
				var message = _localQueue.RemoveMessage();
				CurrentMessageList.Add(message);
			}

			ExtractHandler();
		}

		public void ExtractHandler() {
			_logger.Debug($"Inside:{nameof(LocalQueueOutputProcessor)}_{MethodBase.GetCurrentMethod()}");
			_logger.Debug($"Total of {_instantiateHandler.PluginAssemblyHandlerList.Count} handlers to check.");
			
			var handlerTypeList = _instantiateHandler.PluginAssemblyHandlerList;

			foreach (var message in CurrentMessageList) {
				_logger.Debug($"Finding Handlers for message={message.GetType()}");

				var handlerList = handlerTypeList
					.Where(x => x.Message.FullName != null && x.Message.FullName.Equals(message.GetType().FullName))
					.ToList();

				if (handlerList.Count <= 0) continue;

				InvokeHandler(handlerList, message);
			}
		}

		public void InvokeHandler(List<PluginAssemblyHandlerDto> handlerTypeList, IMessage message) {
			_logger.Debug($"Inside:{nameof(LocalQueueOutputProcessor)}_{MethodBase.GetCurrentMethod()}");
			_logger.Debug($"Total of {handlerTypeList.Count} Handlers found.");

			foreach (var handlerType in handlerTypeList) {
				_logger.Debug($"HandlerInterface to run={handlerType.HandlerInterface}");
				_logger.Debug($"Handler name={handlerType.HandlerClass.Name} full-path={handlerType.HandlerClass.FullName}.");

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