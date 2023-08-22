using System.IO;

namespace Hydrogen;

public class InMemoryObjectContainerIndex<T> {
	private readonly ObjectContainer<T> _objectContainer;
	private readonly long _reservedStreamIndex;

	public InMemoryObjectContainerIndex(ObjectContainer<T> objectContainer, long reservedStreamIndex) {
		Guard.ArgumentNotNull(objectContainer, nameof(objectContainer));
		Guard.Argument(objectContainer.StreamContainer.Header.ReservedStreams > 0, nameof(objectContainer), "The stream container does not have any reserved streams.");
		Guard.ArgumentInRange(reservedStreamIndex, 0, objectContainer.StreamContainer.Header.ReservedStreams - 1, nameof(reservedStreamIndex));
		_objectContainer = objectContainer;
		_reservedStreamIndex = reservedStreamIndex;
		objectContainer.PostItemOperation += IndexItem;
	}

	private void IndexItem(object source, long index, T item, ListOperationType operationType) {
		using var indexStream = _objectContainer.StreamContainer.OpenWrite(_reservedStreamIndex);
		indexStream.Seek(0L, SeekOrigin.Begin);
		
	}

}
