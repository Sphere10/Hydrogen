using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Sphere10.Framework {

	/// <summary>
	/// A list whose items are stored on file-system in a transactional manner and cached in memory for efficiency.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TransactionalList<T> : ObservableExtendedList<T>, ITransactionalList<T> {
		public const int DefaultTransactionalPageSize = 1 << 17;  // 128kb
		public const int DefaultClusterSize = 128;  //1 << 11; // 2kb

		public event EventHandlerEx<object> Loading { add => _clusteredList.Loading += value; remove => _clusteredList.Loading -= value; }
		public event EventHandlerEx<object> Loaded { add => _clusteredList.Loaded += value; remove => _clusteredList.Loaded -= value; }
		public event EventHandlerEx<object> Committing { add => AsBuffer.Committing += value; remove => AsBuffer.Committing -= value; }
		public event EventHandlerEx<object> Committed { add => AsBuffer.Committed += value; remove => AsBuffer.Committed -= value; }
		public event EventHandlerEx<object> RollingBack { add => AsBuffer.RollingBack += value; remove => AsBuffer.RollingBack -= value; }
		public event EventHandlerEx<object> RolledBack { add => AsBuffer.RolledBack += value; remove => AsBuffer.RolledBack -= value; }

		private readonly SynchronizedExtendedList<T> _synchronizedList;
		private readonly ClusteredList<T> _clusteredList;
		private bool _disposed;

		/// <summary>
		/// Creates a <see cref="TransactionalList{T}" /> based on a <see cref="StaticClusteredList{T,TListing}"/>/>
		/// </summary>
		/// <param name="serializer">Serializer for the objects</param>
		/// <param name="filename">File which will contain the serialized objects.</param>
		/// <param name="uncommittedPageFileDir">A working directory which stores transactional pages before committed. Must be same across system restart.</param>
		/// <param name="fileID">A unique ID for this file. Must be the same across system restarts.</param>
		/// <param name="maxStorageBytes">Maximum size file should take.</param>
		/// <param name="maxMemory">How much of the list can be kept in memory at any time</param>
		/// <param name="maxItems">The maximum number of items this file will ever support. <remarks>Avoid <see cref="Int32.MaxValue"/> and give lowest number possible.</remarks> </param>
		/// <param name="readOnly">Whether or not file is opened in readonly mode.</param>
		public TransactionalList(IItemSerializer<T> serializer, string filename, string uncommittedPageFileDir, Guid fileID, long maxStorageBytes, long maxMemory, int maxItems, bool readOnly = false)
			: this(serializer, filename, uncommittedPageFileDir, fileID, DefaultTransactionalPageSize, maxStorageBytes, maxMemory, DefaultClusterSize, maxItems, readOnly) {
		}

		/// <summary>
		/// Creates a <see cref="TransactionalList{T}" /> based on a <see cref="StaticClusteredList{T,TListing}"/>/>.
		/// </summary>
		/// <param name="serializer">Serializer for the objects</param>
		/// <param name="filename">File which will contain the serialized objects.</param>
		/// <param name="uncommittedPageFileDir">A working directory which stores transactional pages before committed. Must be same across system restart.</param>
		/// <param name="fileID">A unique ID for this file. Must be the same across system restarts.</param>
		/// <param name="transactionalPageSizeBytes">Size of transactional page</param>
		/// <param name="maxStorageBytes">Maximum size file should take.</param>		
		/// <param name="maxMemory">How much of the list can be kept in memory at any time</param>
		/// <param name="clusterSize">To support random access reads/writes the file is broken into discontinuous clusters of this size (similar to how disk storage) works. <remarks>Try to fit your average object in 1 cluster for performance. However, spare space in a cluster cannot be used.</remarks> </param>
		/// <param name="maxItems">The maximum count of items this file will ever support. <remarks>Avoid <see cref="Int32.MaxValue"/> and give lowest number possible.</remarks> </param>
		/// <param name="readOnly">Whether or not file is opened in readonly mode.</param>
		public TransactionalList(IItemSerializer<T> serializer, string filename, string uncommittedPageFileDir, Guid fileID, int transactionalPageSizeBytes, long maxStorageBytes, long maxMemory, int clusterSize, int maxItems, bool readOnly = false)
			: this(
					NewStaticClusteredList(
						clusterSize,
						maxItems,
						maxStorageBytes,
						new ExtendedMemoryStream(
							NewTransactionalFileMappedBuffer(
								filename,
								uncommittedPageFileDir,
								fileID,
								transactionalPageSizeBytes,
								maxMemory,
								readOnly,
								out var buffer
							),
							disposeSource: true
						),
						serializer,
						null, // ItemComparer
						out var clusteredList
					),
					buffer
				) {
		}

		/// <summary>
		/// Creates a <see cref="TransactionalList{T}" /> based on a <see cref="DynamicClusteredList{T,TListing}"/>/>.
		/// </summary>
		/// <param name="filename">File which will contain the serialized objects.</param>
		/// <param name="uncommittedPageFileDir">A working directory which stores transactional pages before committed. Must be same across system restart.</param>
		/// <param name="fileID">A unique ID for this file. Must be the same across system restarts.</param>
		/// <param name="maxMemory">How much of the list can be kept in memory at any time</param>
		/// <param name="serializer">Serializer for the objects</param>
		/// <param name="readOnly">Whether or not file is opened in readonly mode.</param>
		public TransactionalList(string filename, string uncommittedPageFileDir, Guid fileID, long maxMemory, IItemSerializer<T> serializer, bool readOnly = false)
			: this(filename, uncommittedPageFileDir, fileID, DefaultTransactionalPageSize, maxMemory, DefaultClusterSize, serializer, readOnly) {
		}

		/// <summary>
		/// Creates a <see cref="TransactionalList{T}" /> based on a <see cref="DynamicClusteredList{T,TListing}"/>/>.
		/// </summary>
		/// <param name="filename">File which will contain the serialized objects.</param>
		/// <param name="uncommittedPageFileDir">A working directory which stores transactional pages before committed. Must be same across system restart.</param>
		/// <param name="fileID">A unique ID for this file. Must be the same across system restarts.</param>
		/// <param name="transactionalPageSizeBytes">Size of transactional page</param>
		/// <param name="maxMemory">How much of the list can be kept in memory at any time</param>
		/// <param name="clusterSize">To support random access reads/writes the file is broken into discontinuous clusters of this size (similar to how disk storage) works. <remarks>Try to fit your average object in 1 cluster for performance. However, spare space in a cluster cannot be used.</remarks> </param>
		/// <param name="serializer">Serializer for the objects</param>
		/// <param name="readOnly">Whether or not file is opened in readonly mode.</param>
		public TransactionalList(string filename, string uncommittedPageFileDir, Guid fileID, int transactionalPageSizeBytes, long maxMemory, int clusterSize, IItemSerializer<T> serializer, bool readOnly = false)
			: this(
					NewDynamicClusteredList(
						clusterSize,
						new ExtendedMemoryStream(
							NewTransactionalFileMappedBuffer(
								filename,
								uncommittedPageFileDir,
								fileID,
								transactionalPageSizeBytes,
								maxMemory,
								readOnly,
								out var buffer
							),
							disposeSource: true
						),
						serializer,
						null, // ItemComparer
						out var clusteredList
					),
					buffer
				) {
		}


		private TransactionalList(ClusteredList<T> clusteredList, TransactionalFileMappedBuffer buffer)
			: base(NewSynchronizedExtendedList(clusteredList, out var synchronizedList)) {
			_disposed = false;
			_clusteredList = clusteredList;
			_synchronizedList = synchronizedList;
			AsBuffer = buffer;
			AsBuffer.Committing += _ => OnCommitting();
			AsBuffer.Committed += _ => OnCommitted();
			AsBuffer.RollingBack += _ => OnRollingBack();
			AsBuffer.RolledBack += _ => OnRolledBack();
		}

		public bool RequiresLoad => _clusteredList.RequiresLoad;

		public ISynchronizedObject<Scope, Scope> ParentSyncObject {
			get => _synchronizedList.ParentSyncObject;
			set => _synchronizedList.ParentSyncObject = value;
		}

		public ReaderWriterLockSlim ThreadLock => _synchronizedList.ThreadLock;

		public string Path => AsBuffer.Path;

		public Guid FileID => AsBuffer.FileID;

		public TransactionalFileMappedBuffer AsBuffer { get; }

		public void Load() => _clusteredList.Load();

		public Scope EnterReadScope() => _synchronizedList.EnterReadScope();

		public Scope EnterWriteScope() => _synchronizedList.EnterWriteScope();

		public void Commit() => AsBuffer.Commit();

		public void Rollback() => AsBuffer.Rollback();

		public void Dispose() {
			AsBuffer?.Dispose();
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

		private static SynchronizedExtendedList<T> NewSynchronizedExtendedList(IExtendedList<T> internalList, out SynchronizedExtendedList<T> result) {
			result = new SynchronizedExtendedList<T>(internalList);
			return result;
		}

		private static ClusteredList<T> NewStaticClusteredList(int clusterDataSize, int maxItems, long maxStorageBytes, Stream stream, IItemSerializer<T> itemSerializer, IEqualityComparer<T> itemComparer, out ClusteredList<T> result) {
			result = new ClusteredList<T>(clusterDataSize, maxItems, maxStorageBytes, stream, itemSerializer, itemComparer);
			return result;
		}

		private static ClusteredList<T> NewDynamicClusteredList(int clusterDataSize, Stream stream, IItemSerializer<T> itemSerializer, IEqualityComparer<T> itemComparer, out ClusteredList<T> result) {
			result = new ClusteredList<T>(clusterDataSize, stream, itemSerializer, itemComparer);
			return result;
		}

		private static TransactionalFileMappedBuffer NewTransactionalFileMappedBuffer(
			string filename,
			string uncommittedPageFileDir,
			Guid fileID,
			int transactionalPageSizeBytes,
			long maxMemory,
			bool readOnly,
			out TransactionalFileMappedBuffer result) {
			result = new TransactionalFileMappedBuffer(filename, uncommittedPageFileDir, fileID, transactionalPageSizeBytes, maxMemory, readOnly) {
				FlushOnDispose = false
			};
			return result;
		}
	}
}