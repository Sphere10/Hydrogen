using System.Collections.Generic;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Processor {
	public interface ILocalQueueOutputProcessor {

		/// <summary>
		/// This event handler fires when a message is put in the LocalQueue.
		/// </summary>
		/// <param name="sender"></param>
		public void OnCommittedLocalQueue(object sender); //TODO Jake: how to deal with this sender object//

		/// <summary>
		/// Process ALL messages: take ALL messages out of the queue one-by-one.
		/// </summary>
		public void ProcessAllMessagesSynchronously();

		public void ExecuteHandler(IList<IMessage> messageList);
	}
}