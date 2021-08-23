using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Processor {
	/// <summary>
	/// IMPORTANT: This class deals exclusively with ALL inputs into the LocalQueue only.
	/// Inputs into and outputs out of the LocalQueue MUST be separated.
	/// </summary>
	public class LocalQueueInput : ILocalQueueInput {

		private readonly IHeliumQueue _localQueue;

		public LocalQueueInput(IHeliumQueue localQueue) {
			_localQueue = localQueue;

			if (_localQueue.RequiresLoad)
				_localQueue.Load();
		}

		public void AddMessageToLocalQueue(IMessage message) {
			_localQueue.AddMessage(message);
		}
	}
}