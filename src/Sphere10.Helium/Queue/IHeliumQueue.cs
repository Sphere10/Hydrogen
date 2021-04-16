using Sphere10.Framework;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue {

	/// <summary>
	/// This Interface is a HeliumQueue.
	/// All HeliumQueues must inherit from this Interface.
	/// The HeliumQueue is a FIFO buffer.
	/// The FIFO approach is critical because that "should" make the queue fast.
	/// The MUST be super quick in taking messages off the queue and adding messages on the queue.
	/// </summary>
	public interface IHeliumQueue : ITransactionalList<IMessage> {

		/// <summary>
		/// Put a message ON the queue.
		/// The message must inserted into the "back" of the queue.
		/// </summary>
		/// <param name="message"></param>
		public void AddMessageToQueue(IMessage message);

		/// <summary>
		/// Take a message OFF the queue.
		/// The message must be taken of the "front" of the queue.
		/// </summary>
		/// <param name="message"></param>
		public void RemoveMessageFromQueue(IMessage message);
	}
}
