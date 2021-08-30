﻿using System;
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
			_logger.Debug($"Inside:{nameof(LocalQueueOutputProcessor)}.{MethodBase.GetCurrentMethod()}");

			ProcessAllMessagesSynchronously();
		}

		public void ProcessAllMessagesSynchronously() {
			_logger.Debug($"Inside:{nameof(LocalQueueOutputProcessor)}.{MethodBase.GetCurrentMethod()}");

			var messageCount = _localQueue.Count;

			for (var i = 0; i < messageCount; i++) {
				var message = _localQueue.RemoveMessage();
				CurrentMessageList.Add(message);
			}

			ExtractHandler();
		}

		public void ExtractHandler() {
			_logger.Debug($"Inside:{nameof(LocalQueueOutputProcessor)}.{MethodBase.GetCurrentMethod()}");
			_logger.Debug($"Total Handlers to check={_instantiateHandler.PluginAssemblyHandlerList.Count}");

			var handlerTypeList = _instantiateHandler.PluginAssemblyHandlerList;

			foreach (var message in CurrentMessageList) {
				_logger.Debug($"Finding Handlers for message={message.GetType()}");

				var handlerList = handlerTypeList
					.Where(x => x.Message.FullName != null && x.Message.FullName.Equals(message.GetType().FullName))
					.ToList();

				if (handlerList.Count <= 0) continue;

				RunHandler(handlerList, message);
			}
		}

		public void RunHandler(List<PluginAssemblyHandlerDto> handlerTypeList, IMessage message) {
			_logger.Debug($"Inside:{nameof(LocalQueueOutputProcessor)}.{MethodBase.GetCurrentMethod()}");

			foreach (var handlerType in handlerTypeList) {
				_logger.Debug($"HandlerInterface to run={handlerType.HandlerInterface}");
				_logger.Debug($"Assembly full path={handlerType.FullPath}");

				var constructor = handlerType.HandlerClass.GetConstructor(Type.EmptyTypes);
				var handler = constructor?.Invoke(null);
				var parameterTypes = new[] { handlerType.Message };
				var handleMethod = handlerType.HandlerClass.GetMethod("Handle", parameterTypes);
				var messageFromTypeInstance = Activator.CreateInstance(handlerType.Message);

				foreach (var propertyInfo in message.GetType().GetProperties()) {
					var property = messageFromTypeInstance?.GetType().GetProperty(propertyInfo.Name, propertyInfo.PropertyType);
					var propertyValue = propertyInfo.GetValue(message);
					property?.SetValue(messageFromTypeInstance, propertyValue, null);
				}

				var parameters = new[] { messageFromTypeInstance };

				handleMethod?.Invoke(handler, parameters);
			}
		}
	}
}