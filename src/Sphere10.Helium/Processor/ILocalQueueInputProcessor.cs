using Sphere10.Helium.Message;

namespace Sphere10.Helium.Processor {
	public interface ILocalQueueInputProcessor {
		public void AddMessageToLocalQueue(IMessage message);
	}
}
