using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Sphere10.Framework {

	public class TransactionalList<T> : ObservableExtendedList<T>, ITransactionalList<T> {
		public const int DefaultTransactionalPageSize = 1 << 17;  // 128kb
		public const int DefaultClusterSize = 128;  //1 << 11; // 2kb

		public event EventHandlerEx<object> Committing { add => AsBuffer.Committing += value; remove => AsBuffer.Committing -= value; }
		public event EventHandlerEx<object> Committed { add => AsBuffer.Committed += value; remove => AsBuffer.Committed -= value; }
		public event EventHandlerEx<object> RollingBack { add => AsBuffer.RollingBack += value; remove => AsBuffer.RollingBack -= value; }
		public event EventHandlerEx<object> RolledBack { add => AsBuffer.RolledBack += value; remove => AsBuffer.RolledBack -= value; }

		private readonly SynchronizedExtendedList<T> _synchronizedList;
		private readonly StreamMappedClusteredListBase<T, ItemListing> _clusteredList;
		private bool _disposed;

		/// <summary>
		/// Creates a <see cref="TransactionalList{T}" /> based on a <see cref="StreamMappedFixedClusteredList{T}"/>/>.
		/// </summary>
		/// <param name="serializer">Serializer for the objects</param>
		/// <param name="filename">File which will contain the serialized objects.</param>
		/// <param name="uncommittedPageFileDir">A working directory which stores transactional pages before comitted. Must be same across system restart.</param>
		/// <param name="fileID">A unique ID for this file. Must be the same across system restarts.</param>
		/// <param name="maxStorageBytes">Maximum size file should take.</remarks>
		/// <param name="memoryCacheBytes">How much of the file is cached in memory. <remarks>This value should (roughly) be a factor of <see cref="transactionalPageSizeBytes"/></remarks></param>
		/// <param name="maxItems">The maximum number of items this file will ever support. <remarks>Avoid <see cref="Int32.MaxValue"/> and give lowest number possible.</remarks> </param>
		/// <param name="readOnly">Whether or not file is opened in readonly mode.</param>
		public TransactionalList(IItemSerializer<T> serializer, string filename, string uncommittedPageFileDir, Guid fileID, int maxStorageBytes, int memoryCacheBytes, int maxItems, bool readOnly = false)
			: this(serializer, filename, uncommittedPageFileDir, fileID, DefaultTransactionalPageSize, maxStorageBytes, memoryCacheBytes, DefaultClusterSize, maxItems, readOnly) {
		}

		/// <summary>
		/// Creates a <see cref="TransactionalList{T}" /> based on a <see cref="StreamMappedFixedClusteredList{T}"/>/>.
		/// </summary>
		/// <param name="serializer">Serializer for the objects</param>
		/// <param name="filename">File which will contain the serialized objects.</param>
		/// <param name="uncommittedPageFileDir">A working directory which stores transactional pages before comitted. Must be same across system restart.</param>
		/// <param name="fileID">A unique ID for this file. Must be the same across system restarts.</param>
		/// <param name="transactionalPageSizeBytes">Size of transactional page</param>
		/// <param name="maxStorageBytes">Maximum size file should take.</remarks>		
		/// <param name="memoryCacheBytes">How much of the file is cached in memory. <remarks>This value should (roughly) be a factor of <see cref="transactionalPageSizeBytes"/></remarks></param>
		/// <param name="clusterSize">To support random access reads/writes the file is broken into discontinuous clusters of this size (similar to how disk storage) works. <remarks>Try to fit your average object in 1 cluster for performance. However, spare space in a cluster cannot be used.</remarks> </param>
		/// <param name="maxItems">The maximum count of items this file will ever support. <remarks>Avoid <see cref="Int32.MaxValue"/> and give lowest number possible.</remarks> </param>
		/// <param name="readOnly">Whether or not file is opened in readonly mode.</param>
		public TransactionalList(IItemSerializer<T> serializer, string filename, string uncommittedPageFileDir, Guid fileID, int transactionalPageSizeBytes, int maxStorageBytes, int memoryCacheBytes, int clusterSize, int maxItems, bool readOnly = false)
			: base(
				NewSynchronizedExtendedList(
					NewStreamMappedFixedClusteredList(
						clusterSize,
						maxItems,
						maxStorageBytes,
						new ExtendedMemoryStream(
							NewTransactionalFileMappedBuffer(
								filename,
								uncommittedPageFileDir,
								fileID,
								transactionalPageSizeBytes,
								Math.Max(1, memoryCacheBytes / transactionalPageSizeBytes),
								readOnly,
								out var buffer
							),
							disposeSource: true
						),
						serializer,
						null, // ItemComparer
						out var clusteredList
					),
					out var synchronizedList
				)
			) {
			_disposed = false;
			_clusteredList = clusteredList;
			_synchronizedList = synchronizedList;
			AsBuffer = buffer;
			AsBuffer.Committing += _ => OnCommitting();
			AsBuffer.Committed += _ => OnCommitted();
			AsBuffer.Committing += _ => OnCommitting();
			AsBuffer.Committed += _ => OnCommitted();
			AsBuffer.RollingBack += _ => OnRollingBack();
			AsBuffer.RolledBack += _ => OnRolledBack();
		}

		/// <summary>
		/// Creates a <see cref="TransactionalList{T}" /> based on a <see cref="StreamMappedDynamicClusteredList{T}"/>/>.
		/// </summary>
		/// <param name="filename">File which will contain the serialized objects.</param>
		/// <param name="uncommittedPageFileDir">A working directory which stores transactional pages before comitted. Must be same across system restart.</param>
		/// <param name="fileID">A unique ID for this file. Must be the same across system restarts.</param>
		/// <param name="memoryCacheBytes">How much of the file is cached in memory. <remarks>This value should (roughly) be a factor of <see cref="transactionalPageSizeBytes"/></remarks></param>
		/// <param name="serializer">Serializer for the objects</param>
		/// <param name="readOnly">Whether or not file is opened in readonly mode.</param>
		public TransactionalList(string filename, string uncommittedPageFileDir, Guid fileID, int memoryCacheBytes, IItemSerializer<T> serializer, bool readOnly = false)
			: this(filename, uncommittedPageFileDir, fileID, DefaultTransactionalPageSize, memoryCacheBytes, DefaultClusterSize, serializer, readOnly) {
		}

		/// <summary>
		/// Creates a <see cref="TransactionalList{T}" /> based on a <see cref="StreamMappedDynamicClusteredList{T}"/>/>.
		/// </summary>
		/// <param name="filename">File which will contain the serialized objects.</param>
		/// <param name="uncommittedPageFileDir">A working directory which stores transactional pages before comitted. Must be same across system restart.</param>
		/// <param name="fileID">A unique ID for this file. Must be the same across system restarts.</param>
		/// <param name="transactionalPageSizeBytes">Size of transactional page</param>
		/// <param name="memoryCacheBytes">How much of the file is cached in memory. <remarks>This value should (roughly) be a factor of <see cref="transactionalPageSizeBytes"/></remarks></param>
		/// <param name="clusterSize">To support random access reads/writes the file is broken into discontinuous clusters of this size (similar to how disk storage) works. <remarks>Try to fit your average object in 1 cluster for performance. However, spare space in a cluster cannot be used.</remarks> </param>
		/// <param name="serializer">Serializer for the objects</param>
		/// <param name="readOnly">Whether or not file is opened in readonly mode.</param>
		public TransactionalList(string filename, string uncommittedPageFileDir, Guid fileID, int transactionalPageSizeBytes, int memoryCacheBytes, int clusterSize, IItemSerializer<T> serializer, bool readOnly = false)
			: base(
				NewSynchronizedExtendedList(
					NewStreamMappedDynamicClusteredList(
						clusterSize,
						new ExtendedMemoryStream(
							NewTransactionalFileMappedBuffer(
								filename,
								uncommittedPageFileDir,
								fileID,
								transactionalPageSizeBytes,
								Math.Max(1, memoryCacheBytes / transactionalPageSizeBytes),
								readOnly,
								out var buffer
							),
							disposeSource: true
						),
						serializer,
						null, // ItemComparer
						out var clusteredList
					),
					out var synchronizedList)
			) {
			_disposed = false;
			_clusteredList = clusteredList;
			_synchronizedList = synchronizedList;
			AsBuffer = buffer;
			AsBuffer.Committing += _ => OnCommitting();
			AsBuffer.Committed += _ => OnCommitted();
			AsBuffer.Committing += _ => OnCommitting();
			AsBuffer.Committed += _ => OnCommitted();
			AsBuffer.RollingBack += _ => OnRollingBack();
			AsBuffer.RolledBack += _ => OnRolledBack();
		}

		public bool RequiresLoad => _clusteredList.RequiresLoad;

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

		private static StreamMappedFixedClusteredList<T> NewStreamMappedFixedClusteredList(int clusterDataSize, int maxItems, int maxStorageBytes, Stream stream, IItemSerializer<T> itemSerializer, IEqualityComparer<T> itemComparer, out StreamMappedFixedClusteredList<T> result) {
			result = new StreamMappedFixedClusteredList<T>(clusterDataSize, maxItems, maxStorageBytes, stream, itemSerializer, itemComparer);
			return result;
		}

		private static StreamMappedDynamicClusteredList<T> NewStreamMappedDynamicClusteredList(int clusterDataSize, Stream stream, IItemSerializer<T> itemSerializer, IEqualityComparer<T> itemComparer, out StreamMappedDynamicClusteredList<T> result) {
			result = new StreamMappedDynamicClusteredList<T>(clusterDataSize, stream, itemSerializer, itemComparer);
			return result;
		}

		private static TransactionalFileMappedBuffer NewTransactionalFileMappedBuffer(
			string filename,
			string uncommittedPageFileDir,
			Guid fileID,
			int transactionalPageSizeBytes,
			int inMemPages,
			bool readOnly,
			out TransactionalFileMappedBuffer result) {
			result = new TransactionalFileMappedBuffer(filename, uncommittedPageFileDir, fileID, transactionalPageSizeBytes, inMemPages, readOnly) {
				FlushOnDispose = false
			};
			return result;
		}
	}
}