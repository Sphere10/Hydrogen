using Sphere10.Helium.Message;

namespace Sphere10.Helium.Processor {
	public interface ILocalQueueProcessor {

		/// <summary>
		/// This event handler fires when a message is put in the LocalQueue by the Bus.
		/// </summary>
		/// <param name="sender"></param>
		public void OnCommittedLocalQueue(object sender); //TODO Jake: how to deal with this sender object//

		/// <summary>
		/// This event handler fires when a message is put in the LocalQueue by the Bus.
		/// </summary>
		/// <param name="sender"></param>
		public void OnCommittedProcessingQueue(object sender); //TODO Jake: how to deal with this sender object//

		/// <summary>
		/// Once a message is added to the LocalQueue it is moved RELIABLY into the ProcessingQueue for processing
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public void MoveFirstMessageFromLocalToProcessing();

		public void InsertMessageInLocalQueue(IMessage message);

		public IMessage TakeFirstMessageOutOfLocalQueue();
	}
}
