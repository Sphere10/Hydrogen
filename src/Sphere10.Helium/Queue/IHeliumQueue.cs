using Sphere10.Framework;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue {

	/// <summary>
	/// This Interface is a HeliumQueue.
	/// All HeliumQueues must inherit from this Interface.
	/// The HeliumQueue is a FIFO buffer.
	/// The FIFO approach is critical because that makes the queue fast.
	/// This MUST be super quick in taking messages off the queue and adding messages on the queue.
	/// </summary>
	public interface IHeliumQueue : ITransactionalList<IMessage> {

		/// <summary>
		/// PUTS a message ON the queue.
		/// The message must be inserted into the "back" of the queue.
		/// </summary>
		/// <param name="message"></param>
		public void AddMessage(IMessage message);

		/// <summary>
		/// DELETES a message FROM the queue.
		/// The message must be taken off the "front" of the queue.
		/// </summary>
		/// <param name="message"></param>
		public void DeleteMessage(IMessage message);

		/// <summary>
		/// RETRIEVES a message FROM the queue.
		/// The message must be taken off the "front" of the queue.
		/// </summary>
		/// <param name="message"></param>
		public IMessage RetrieveMessage();
	}
}
