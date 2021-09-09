using System;
using Sphere10.Helium.Processor;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.HeliumNode {
	public class ConfigureHeliumEndpoint : IConfigureHeliumEndpoint{
		private readonly ILocalQueueInputProcessor _localQueueInputProcessor;
		private readonly IPrivateQueueInputProcessor _privateQueueProcessor;

		public ConfigureHeliumEndpoint(ILocalQueueInputProcessor localQueueInputProcessor, IPrivateQueueInputProcessor privateQueueProcessor) {
			_localQueueInputProcessor = localQueueInputProcessor;
			_privateQueueProcessor = privateQueueProcessor;
		}

		public void SetupEndpoint(HeliumNodeSettings endPointSettings) {
			if(endPointSettings.FlushLocalQueueOnStartup) _localQueueInputProcessor.FlushLocalQueue();
			if(endPointSettings.FlushPrivateQueueOnStartup) _privateQueueProcessor.FlushPrivateQueue();
		}

		public void CheckSettings() {
			var localQueueSetting = new LocalQueueSettings();

			if (localQueueSetting.InputMessageBatchSize < localQueueSetting.AmountOfProcessingThreads)
				throw new ArgumentOutOfRangeException(nameof(localQueueSetting.AmountOfProcessingThreads), "AmountOfProcessingThreads CANNOT be bigger than BatchSize? Doesn't make sense.");
		}
	}
}