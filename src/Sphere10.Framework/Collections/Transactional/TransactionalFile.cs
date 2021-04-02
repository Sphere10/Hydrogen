using System;
using System.IO;

namespace Sphere10.Framework {

	public class TransactionalFile<T> : ExtendedListDecorator<T>, ITransactionalFile, IDisposable {
		public const int DefaultTransactionalPageSize = 1 << 17;  // 128kb
		public const int DefaultClusterSize = 1 << 11; // 2kb
	
		private readonly TransactionalFileMappedBuffer _buffer;

		/// <summary>
		/// Creates a <see cref="TransactionalFile{T}"/>.
		/// </summary>
		/// <param name="serializer">Serializer for the objects</param>
		/// <param name="filename">File which will contain the serialized objects.</param>
		/// <param name="uncommittedPageFileDir">A working directory which stores transactional pages before comitted. Must be same across system restart.</param>
		/// <param name="fileID">A unique ID for this file. Must be the same across system restarts.</param>
		/// <param name="memoryCacheBytes">How much of the file is cached in memory. <remarks>This value should (roughly) be a factor of <see cref="transactionalPageSizeBytes"/></remarks></param>
		/// <param name="maxItems">The maximum number of items this file will ever support. <remarks>Avoid <see cref="Int32.MaxValue"/> and give lowest number possible.</remarks> </param>
		/// <param name="readOnly">Whether or not file is opened in readonly mode.</param>
		public TransactionalFile(IObjectSerializer<T> serializer, string filename, string uncommittedPageFileDir, Guid fileID, int memoryCacheBytes, int maxItems, bool readOnly = false)
			: this(serializer, filename, uncommittedPageFileDir, fileID, DefaultTransactionalPageSize, memoryCacheBytes, DefaultClusterSize, maxItems, readOnly) {
		}



		/// <summary>
		/// Creates a <see cref="TransactionalFile{T}"/>.
		/// </summary>
		/// <param name="serializer">Serializer for the objects</param>
		/// <param name="filename">File which will contain the serialized objects.</param>
		/// <param name="uncommittedPageFileDir">A working directory which stores transactional pages before comitted. Must be same across system restart.</param>
		/// <param name="fileID">A unique ID for this file. Must be the same across system restarts.</param>
		/// <param name="transactionalPageSizeBytes">Size of transactional page</param>
		/// <param name="memoryCacheBytes">How much of the file is cached in memory. <remarks>This value should (roughly) be a factor of <see cref="transactionalPageSizeBytes"/></remarks></param>
		/// <param name="clusterSize">To support random access reads/writes the file is broken into discontinuous clusters of this size (similar to how disk storage) works. <remarks>Try to fit your average object in 1 cluster for performance. However, spare space in a cluster cannot be used.</remarks> </param>
		/// <param name="maxItems">The maximum number of items this file will ever support. <remarks>Avoid <see cref="Int32.MaxValue"/> and give lowest number possible.</remarks> </param>
		/// <param name="readOnly">Whether or not file is opened in readonly mode.</param>
		public TransactionalFile(IObjectSerializer<T> serializer, string filename, string uncommittedPageFileDir, Guid fileID, int transactionalPageSizeBytes, int memoryCacheBytes, int clusterSize, int maxItems, bool readOnly = false)
			: base(
				new StreamMappedFixedClusteredList<T>(
					clusterSize,
					maxItems,
					serializer,
					new ExtendedMemoryStream(
						NewTransactionalFileMappedBuffer(
							filename,
							uncommittedPageFileDir,
							fileID,
							transactionalPageSizeBytes,
							Math.Max(1, memoryCacheBytes / transactionalPageSizeBytes),
							readOnly,
							out var buffer
						)
					)
				)
			) {
			_buffer = buffer;
		}

		
		public void Commit() => _buffer.Commit();

		public void Rollback() => _buffer.Rollback();

		public string Path => _buffer.Path;

		public Guid FileID => _buffer.FileID;

		public TransactionalFileMappedBuffer AsBuffer => _buffer;

		private static TransactionalFileMappedBuffer NewTransactionalFileMappedBuffer(
			string filename,
			string uncommittedPageFileDir,
			Guid fileID,
			int transactionalPageSizeBytes,
			int inMemPages,
			bool readOnly,
			out TransactionalFileMappedBuffer result) {
			result = new TransactionalFileMappedBuffer(filename, uncommittedPageFileDir, fileID, transactionalPageSizeBytes, inMemPages, readOnly);
			return result;
		}

		public void Dispose() {
			_buffer?.Dispose();
			AsBuffer?.Dispose();
		}
	}

}