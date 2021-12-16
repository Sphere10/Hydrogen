namespace Sphere10.Helium.Processor {
	public interface ILocalQueueOutputProcessor {

		public void OnCommittedLocalQueue(object sender); //TODO Jake: how to deal with this sender object//

		public void ProcessAllMessagesInQueue();
	}
}