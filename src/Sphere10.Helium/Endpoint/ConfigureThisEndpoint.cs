using Sphere10.Helium.Processor;

namespace Sphere10.Helium.Endpoint {
	public class ConfigureThisEndpoint : IConfigureThisEndpoint{
		private readonly ILocalQueueInputProcessor _localQueueInputProcessor;
		private readonly IPrivateQueueInputProcessor _privateQueueProcessor;

		public ConfigureThisEndpoint(ILocalQueueInputProcessor localQueueInputProcessor, IPrivateQueueInputProcessor privateQueueProcessor) {
			_localQueueInputProcessor = localQueueInputProcessor;
			_privateQueueProcessor = privateQueueProcessor;
		}

		public void SetupEndpoint(EndPointSettings endPointSettings) {
			if(endPointSettings.FlushLocalQueueOnStartup) _localQueueInputProcessor.FlushLocalQueue();
			if(endPointSettings.FlushPrivateQueueOnStartup) _privateQueueProcessor.FlushPrivateQueue();
		}
	}
}