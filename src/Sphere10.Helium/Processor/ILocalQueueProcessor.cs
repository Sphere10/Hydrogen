using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Processor {
	public interface ILocalQueueProcessor {

		protected ILocalQueue LocalQueue { get; set; }

		protected IProcessingQueue ProcessingQueue { get; set; }

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
		public bool MoveMessageFromLocalToProcessing(IMessage message);
		
	}
}
