using System;
using System.IO;
using System.Threading.Tasks;

namespace Hydrogen;

internal class MetaDataStreamBase : ILoadable, IDisposable {

	public event EventHandlerEx<object> Loading;
	public event EventHandlerEx<object> Loaded;

	private readonly long _reservedStreamIndex;
	private readonly long _streamOffset;
	private bool _attached;


	public MetaDataStreamBase(ObjectContainer objectContainer, long reservedStreamIndex, long offset) {
		Container = objectContainer;
		Guard.Ensure(objectContainer.RequiresLoad, "Object Container must not be loaded before creating index");
		_reservedStreamIndex = reservedStreamIndex;
		_streamOffset = offset;
		_attached = false;

		objectContainer.Loaded += ObjectContainer_Loaded;
		objectContainer.Clearing += ObjectContainer_Clearing;
		objectContainer.Cleared += ObjectContainer_Cleared;
	}

	public bool RequiresLoad => Container.RequiresLoad || !_attached;

	protected ObjectContainer Container { get; }

	private Stream _stream;
	protected Stream Stream { 
		get {
			CheckAttached();
			return _stream;
		}
		private set => _stream = value;
	}

	public void Load() {
		if (Container.RequiresLoad)
			throw new InvalidOperationException("Object Container is not loaded");

		if (!_attached)
			Attach();
	}

	public Task LoadAsync() => throw new NotSupportedException();

	public void Dispose() {
		if (_attached)
			Detach();
	}

	protected virtual void OnLoading() {
	}

	protected virtual void OnLoaded() {
	}

	protected void CheckAttached() {
		if (!_attached)
			throw new InvalidOperationException("Index is not attached");
	}

	protected void CheckNotAttached() {
		if (_attached)
			throw new InvalidOperationException("Index is already attached");
	}

	private void Attach() {
		CheckNotAttached();
		NotifyLoading();
		Guard.Ensure(Container.StreamContainer.Header.ReservedStreams > 0, "Stream Container has no reserved streams available");
		Guard.Ensure(_reservedStreamIndex < Container.StreamContainer.Header.ReservedStreams, $"Stream at index {_reservedStreamIndex} is not reserved");
		using (Container.StreamContainer.EnterAccessScope()) {
			_attached = true;

			// Open the stream used by the index. No access scope is acquired for the stream
			// and thus all use of the index must take place within an explicit access scope.
			Stream =
				Container
				.StreamContainer
				.Open(_reservedStreamIndex, false, false)
				.AsBounded(_streamOffset, long.MaxValue, allowInnerResize: true);

			// Ensures the stream is at least as long as the offset (the space prior to offset can
			// be used to store header information (i.e. factory info to decide what type of index to load)
			if (Stream.Position < 0)
				Stream.SetLength(0);

		}
		NotifyLoaded();
	}

	private void ObjectContainer_Loaded(object arg) {
		if (!_attached)
			Load();
	}

	private void ObjectContainer_Clearing(object arg) {
		CheckAttached();
		Detach();
	}

	private void ObjectContainer_Cleared(object arg) {
		CheckNotAttached();
		Attach();
	}

	private void Detach() {
		CheckAttached();
		Stream.Dispose();
		Stream = null;
		_attached = false;
	}

	private void NotifyLoading() {
		OnLoading();
		Loading?.Invoke(this);
	}

	private void NotifyLoaded() {
		OnLoaded();
		Loaded?.Invoke(this);
	}

}
