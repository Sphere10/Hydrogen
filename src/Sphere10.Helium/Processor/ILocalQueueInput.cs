using Sphere10.Helium.Message;

namespace Sphere10.Helium.Processor {
	public interface ILocalQueueInput {
		public void AddMessageToLocalQueue(IMessage message);
	}
}
