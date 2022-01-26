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
		public const int DefaultClusterSize = 1 << 17;  // 128kb
		public const int DefaultMaxMemory = 10 * (1 << 20);// 10mb

		public event EventHandlerEx<object> Loading { add => _transactionalBuffer.Loading += value; remove => _transactionalBuffer.Loading -= value; }
		public event EventHandlerEx<object> Loaded { add => _transactionalBuffer.Loaded += value; remove => _transactionalBuffer.Loaded -= value; }
		public event EventHandlerEx<object> Committing { add => _transactionalBuffer.Committing += value; remove => _transactionalBuffer.Committing -= value; }
		public event EventHandlerEx<object> Committed { add => _transactionalBuffer.Committed += value; remove => _transactionalBuffer.Committed -= value; }
		public event EventHandlerEx<object> RollingBack { add => _transactionalBuffer.RollingBack += value; remove => _transactionalBuffer.RollingBack -= value; }
		public event EventHandlerEx<object> RolledBack { add => _transactionalBuffer.RolledBack += value; remove => _transactionalBuffer.RolledBack -= value; }

		private readonly TransactionalFileMappedBuffer _transactionalBuffer;
		private readonly SynchronizedExtendedList<T> _itemList;
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
		public TransactionalList(string filename, string uncommittedPageFileDir, IItemSerializer<T> serializer, IEqualityComparer<T> comparer = null, int transactionalPageSizeBytes = DefaultTransactionalPageSize, long maxMemory = DefaultMaxMemory, int clusterSize = DefaultClusterSize, bool readOnly = false) {
			Guard.ArgumentNotNull(filename, nameof(filename));
			Guard.ArgumentNotNull(uncommittedPageFileDir, nameof(uncommittedPageFileDir));
			var fileID = new Guid(Hashers.Hash(CHF.SHA2_256, Tools.FileSystem.GetCaseCorrectFilePath(filename).ToBase64().ToAsciiByteArray()).Take(16).ToArray());
			_disposed = false;

			_transactionalBuffer = new TransactionalFileMappedBuffer(filename, uncommittedPageFileDir, fileID, transactionalPageSizeBytes, maxMemory, readOnly) {
				FlushOnDispose = false
			};
			_transactionalBuffer.Committing += _ => OnCommitting();
			_transactionalBuffer.Committed += _ => OnCommitted();
			_transactionalBuffer.RollingBack += _ => OnRollingBack();
			_transactionalBuffer.RolledBack += _ => OnRolledBack();

			var clusteredList = new ClusteredList<T>(
				new ExtendedMemoryStream(
					_transactionalBuffer,
					disposeSource: true
				),
				clusterSize,
				serializer,
				comparer
			);

			_itemList = new SynchronizedExtendedList<T>(clusteredList);
			InternalCollection = _itemList;
		}

		public bool RequiresLoad => _transactionalBuffer.RequiresLoad;

		public ISynchronizedObject<Scope, Scope> ParentSyncObject {
			get => _itemList.ParentSyncObject;
			set => _itemList.ParentSyncObject = value;
		}

		public ReaderWriterLockSlim ThreadLock => _itemList.ThreadLock;

		public string Path => _transactionalBuffer.Path;

		public Guid FileID => _transactionalBuffer.FileID;

		public TransactionalFileMappedBuffer AsBuffer => _transactionalBuffer;

		public void Load() => _transactionalBuffer.Load();

		public Scope EnterReadScope() => _itemList.EnterReadScope();

		public Scope EnterWriteScope() => _itemList.EnterWriteScope();

		public void Commit() => _transactionalBuffer.Commit();

		public void Rollback() => _transactionalBuffer.Rollback();

		public void Dispose() {
			_transactionalBuffer?.Dispose();
			_disposed = true;
		}

		protected override void OnAccessing(EventTraits eventType) {
			if (_disposed)
				throw new InvalidOperationException("Queue has been disposed");
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