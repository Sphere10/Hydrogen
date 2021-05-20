using Sphere10.Helium.Message;

namespace Sphere10.Helium.Processor {
	public interface IQueueManager {
		void FirstIn(string destination, IMessage message);

		void LastOut(IMessage message);

		void TakeThisMessageOffQueue(IMessage message);

		void PersistQueue();
	}
}
