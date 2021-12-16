using Sphere10.Framework;

namespace Sphere10.Helium.Processor {
	public class PrivateQueueInputProcessor : IPrivateQueueInputProcessor{
		private readonly ILogger _logger;

		public PrivateQueueInputProcessor(ILogger logger) {
			_logger = logger;
		}

		public void FlushPrivateQueue() {
			//All good do nothing for now.
		}
	}
}