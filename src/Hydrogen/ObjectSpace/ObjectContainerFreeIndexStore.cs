
namespace Hydrogen.ObjectSpace.MetaData;

internal class ObjectContainerFreeIndexStore : MetaDataStreamBase, IMetaDataStack {
	
	private IStack<long> _freeIndexStack;
	public ObjectContainerFreeIndexStore(ObjectContainer objectContainer, long reservedStreamIndex, long offset) 
		: base(objectContainer, reservedStreamIndex, offset) {
		_freeIndexStack = null;
		objectContainer.PostItemOperation += ObjectContainer_PostItemOperation;
	}

	private void ObjectContainer_PostItemOperation(object source, long index, object item, ObjectContainerOperationType operationType) {
		// When an object is reaped, we remember the index so it can be re-used later
		if (operationType == ObjectContainerOperationType.Reap) {
			Stack.Push(index);
		}
	}

	public IStack<long> Stack { 
		get {
			CheckAttached();
			return _freeIndexStack;
		}
	}

	protected override void OnLoaded() {
		// the free index stack is an in-stream stack at all times (no memory overhead)
		_freeIndexStack = new StreamPagedList<long>(
			PrimitiveSerializer<long>.Instance,
			Stream,
			Endianness.LittleEndian,
			false,
			true
		).AsStack();
	}
}
