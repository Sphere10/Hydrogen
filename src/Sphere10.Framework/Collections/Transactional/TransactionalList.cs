using System;
using System.IO;

namespace Sphere10.Framework {

	public class TransactionalList<T> : ObservableExtendedList<T>, ITransactionalList<T> {
		public const int DefaultTransactionalPageSize = 1 << 17;  // 128kb
		public const int DefaultClusterSize = 128;  //1 << 11; // 2kb

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
		public TransactionalList(IObjectSerializer<T> serializer, string filename, string uncommittedPageFileDir, Guid fileID, int maxStorageBytes, int memoryCacheBytes, int maxItems, bool readOnly = false)
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
		public TransactionalList(IObjectSerializer<T> serializer, string filename, string uncommittedPageFileDir, Guid fileID, int transactionalPageSizeBytes, int maxStorageBytes, int memoryCacheBytes, int clusterSize, int maxItems, bool readOnly = false)
			: base(
				new StreamMappedFixedClusteredList<T>(
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
					serializer
				)
			) {
			AsBuffer = buffer;
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
		public TransactionalList(string filename, string uncommittedPageFileDir, Guid fileID, int memoryCacheBytes, IObjectSerializer<T> serializer, bool readOnly = false)
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
		public TransactionalList(string filename, string uncommittedPageFileDir, Guid fileID, int transactionalPageSizeBytes, int memoryCacheBytes, int clusterSize, IObjectSerializer<T> serializer, bool readOnly = false)
			: base(
				new StreamMappedDynamicClusteredList<T>(
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
					serializer
				)
			) {
			AsBuffer = buffer;
		}

		public bool RequiresLoad => ((ILoadable)base.InnerCollection).RequiresLoad;

		public void Load() => ((ILoadable)InnerCollection).Load();

		public void Commit() => AsBuffer.Commit();

		public void Rollback() => AsBuffer.Rollback();

		public string Path => AsBuffer.Path;

		public Guid FileID => AsBuffer.FileID;

		public TransactionalFileMappedBuffer AsBuffer { get; }

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


		public void Dispose() {
			AsBuffer?.Dispose();
		}
	}

}