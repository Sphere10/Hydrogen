using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sphere10.Framework {

	/// <summary>
	/// A list whose items are stored on a file and which has ACID transactional commit/rollback capability as well as built-in memory caching for efficiency. There are
	/// no restrictions on this list, items may be added, mutated and removed arbitrarily. This class is essentially a light-weight database.
	/// </summary>
	/// <typeparam name="T">Type of item</typeparam>
	public class TransactionalList<T> : ObservableExtendedList<T>, ITransactionalList<T> {
		public const int DefaultTransactionalPageSize = 1 << 18;  // 256kb
		public const int DefaultClusterSize = 256;   // 256b
		public const int DefaultMaxMemory = int.MaxValue;// 10mb

		public event EventHandlerEx<object> Loading { add => _transactionalBuffer.Loading += value; remove => _transactionalBuffer.Loading -= value; }
		public event EventHandlerEx<object> Loaded { add => _transactionalBuffer.Loaded += value; remove => _transactionalBuffer.Loaded -= value; }
		public event EventHandlerEx<object> Committing { add => _transactionalBuffer.Committing += value; remove => _transactionalBuffer.Committing -= value; }
		public event EventHandlerEx<object> Committed { add => _transactionalBuffer.Committed += value; remove => _transactionalBuffer.Committed -= value; }
		public event EventHandlerEx<object> RollingBack { add => _transactionalBuffer.RollingBack += value; remove => _transactionalBuffer.RollingBack -= value; }
		public event EventHandlerEx<object> RolledBack { add => _transactionalBuffer.RolledBack += value; remove => _transactionalBuffer.RolledBack -= value; }

		private readonly TransactionalFileMappedBuffer _transactionalBuffer;
		private readonly ClusteredList<T> _clustered;
		private readonly SynchronizedExtendedList<T> _items;
		private bool _disposed;


		/// <summary>
		/// Creates a <see cref="TransactionalList{T}" />.
		/// </summary>
		/// <param name="filename">File which will contain the serialized objects.</param>
		/// <param name="uncommittedPageFileDir">A working directory which stores uncommitted transactional pages. Must be the same across system restarts for transaction resumption.</param>
		/// <param name="serializer">Serializer for the objects</param>
		/// <param name="comparer"></param>
		/// <param name="transactionalPageSizeBytes">Size of transactional page</param>
		/// <param name="maxMemory">How much of the list can be kept in memory at any time</param>
		/// <param name="clusterSize">To support random access reads/writes the file is broken into discontinuous clusters of this size (similar to how disk storage) works. <remarks>Try to fit your average object in 1 cluster for performance. However, spare space in a cluster cannot be used.</remarks> </param>
		/// <param name="readOnly">Whether or not file is opened in readonly mode.</param>
		public TransactionalList(string filename, string uncommittedPageFileDir, IItemSerializer<T> serializer, IEqualityComparer<T> comparer = null, int transactionalPageSizeBytes = DefaultTransactionalPageSize, long maxMemory = DefaultMaxMemory, int clusterSize = DefaultClusterSize, ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, Endianness endianness = Endianness.LittleEndian, bool readOnly = false) {
			Guard.ArgumentNotNull(filename, nameof(filename));
			Guard.ArgumentNotNull(uncommittedPageFileDir, nameof(uncommittedPageFileDir));

			_disposed = false;

			_transactionalBuffer = new TransactionalFileMappedBuffer(filename, uncommittedPageFileDir, transactionalPageSizeBytes, maxMemory, readOnly);
			_transactionalBuffer.Committing += _ => OnCommitting();
			_transactionalBuffer.Committed += _ => OnCommitted();
			_transactionalBuffer.RollingBack += _ => OnRollingBack();
			_transactionalBuffer.RolledBack += _ => OnRolledBack();


			// NOTE: needs removal
			if (_transactionalBuffer.RequiresLoad)
				_transactionalBuffer.Load();

			_clustered = new ClusteredList<T>(
				new ExtendedMemoryStream(
					_transactionalBuffer,
					disposeSource: true
				),
				clusterSize,
				serializer,
				comparer,
				policy,
				endianness
			);

			_items = new SynchronizedExtendedList<T>(_clustered);
			InternalCollection = _items;
		}

		public bool RequiresLoad => _transactionalBuffer.RequiresLoad;

		public ISynchronizedObject<Scope, Scope> ParentSyncObject {
			get => _items.ParentSyncObject;
			set => _items.ParentSyncObject = value;
		}

		public ReaderWriterLockSlim ThreadLock => _items.ThreadLock;

		public string Path => _transactionalBuffer.Path;

		public Guid FileID => _transactionalBuffer.FileID;

		public TransactionalFileMappedBuffer AsBuffer => _transactionalBuffer;

		public IClusteredStorage Storage => _clustered.Storage;

		public void Load() => _transactionalBuffer.Load();

		public Scope EnterReadScope() => _items.EnterReadScope();

		public Scope EnterWriteScope() => _items.EnterWriteScope();

		public void Commit() => _transactionalBuffer.Commit();

		public void Rollback() => _transactionalBuffer.Rollback();

		public void Dispose() {
			_transactionalBuffer?.Dispose();
			_disposed = true;
		}

		protected override void OnAccessing(EventTraits eventType) {
			if (_disposed)
				throw new InvalidOperationException($"{GetType().Name} has been disposed");
		}

		protected virtual void OnCommitting() {
		}

		protected virtual void OnCommitted() {
		}

		protected virtual void OnRollingBack() {
		}

		protected virtual void OnRolledBack() {
		}

	}
}