using System;
using System.Collections;
using System.Collections.Generic;
using Sphere10.Framework;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue {
	public class ProcessingQueue : TransactionalList<IMessage>, IHeliumQueue {
		private readonly int _count = 1;
		private readonly int _count1 = 1;
		private readonly int _count2 = 1;

		public ProcessingQueue(LocalQueueSettings localQueueSettings)
			: base(
				new BinaryFormattedSerializer<IMessage>(),
				localQueueSettings.Path,
				localQueueSettings.TempDirPath,
				localQueueSettings.FileId,
				localQueueSettings.TransactionalPageSizeBytes,
				localQueueSettings.MaxStorageSizeBytes,
				localQueueSettings.AllocatedMemory,
				localQueueSettings.ClusterSize,
				localQueueSettings.MaxItems
			) {
		}

		public override IEnumerator<IMessage> GetEnumerator() {
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		void ICollection<IMessage>.Add(IMessage item) {
			throw new NotImplementedException();
		}

		void IExtendedCollection<IMessage>.Clear() {
			throw new NotImplementedException();
		}

		bool IExtendedCollection<IMessage>.Contains(IMessage item) {
			throw new NotImplementedException();
		}

		void IExtendedCollection<IMessage>.CopyTo(IMessage[] array, int arrayIndex) {
			throw new NotImplementedException();
		}

		bool IExtendedCollection<IMessage>.Remove(IMessage item) {
			throw new NotImplementedException();
		}

		void IExtendedCollection<IMessage>.Add(IMessage item) {
			throw new NotImplementedException();
		}

		public override void AddRange(IEnumerable<IMessage> items) {
			throw new NotImplementedException();
		}

		bool IWriteOnlyExtendedCollection<IMessage>.Remove(IMessage item) {
			throw new NotImplementedException();
		}

		int IExtendedCollection<IMessage>.Count => _count2;

		public override IEnumerable<bool> RemoveRange(IEnumerable<IMessage> items) {
			throw new NotImplementedException();
		}

		void IWriteOnlyExtendedCollection<IMessage>.Clear() {
			throw new NotImplementedException();
		}

		void ICollection<IMessage>.Clear() {
			throw new NotImplementedException();
		}

		bool ICollection<IMessage>.Contains(IMessage item) {
			throw new NotImplementedException();
		}

		public override IEnumerable<bool> ContainsRange(IEnumerable<IMessage> items) {
			throw new NotImplementedException();
		}

		void IReadOnlyExtendedCollection<IMessage>.CopyTo(IMessage[] array, int arrayIndex) {
			throw new NotImplementedException();
		}

		bool IReadOnlyExtendedCollection<IMessage>.Contains(IMessage item) {
			throw new NotImplementedException();
		}

		void ICollection<IMessage>.CopyTo(IMessage[] array, int arrayIndex) {
			throw new NotImplementedException();
		}

		void IWriteOnlyExtendedCollection<IMessage>.Add(IMessage item) {
			throw new NotImplementedException();
		}

		bool ICollection<IMessage>.Remove(IMessage item) {
			throw new NotImplementedException();
		}

		int ICollection<IMessage>.Count => _count;

		public override bool IsReadOnly { get; }

		int IReadOnlyCollection<IMessage>.Count => _count1;

		int IList<IMessage>.IndexOf(IMessage item) {
			throw new NotImplementedException();
		}

		void IExtendedList<IMessage>.Insert(int index, IMessage item) {
			throw new NotImplementedException();
		}

		void IExtendedList<IMessage>.RemoveAt(int index) {
			throw new NotImplementedException();
		}

		int IExtendedList<IMessage>.IndexOf(IMessage item) {
			throw new NotImplementedException();
		}

		public new IEnumerable<int> IndexOfRange(IEnumerable<IMessage> items) {
			throw new NotImplementedException();
		}

		public new IMessage Read(int index) {
			throw new NotImplementedException();
		}

		public new IEnumerable<IMessage> ReadRange(int index, int count) {
			throw new NotImplementedException();
		}

		public override void Update(int index, IMessage item) {
			throw new NotImplementedException();
		}

		public override void UpdateRange(int index, IEnumerable<IMessage> items) {
			throw new NotImplementedException();
		}

		void IWriteOnlyExtendedList<IMessage>.Insert(int index, IMessage item) {
			throw new NotImplementedException();
		}

		public override void InsertRange(int index, IEnumerable<IMessage> items) {
			throw new NotImplementedException();
		}

		void IWriteOnlyExtendedList<IMessage>.RemoveAt(int index) {
			throw new NotImplementedException();
		}

		public override void RemoveRange(int index, int count) {
			throw new NotImplementedException();
		}

		void IList<IMessage>.Insert(int index, IMessage item) {
			throw new NotImplementedException();
		}

		void IList<IMessage>.RemoveAt(int index) {
			throw new NotImplementedException();
		}

		public new IMessage this[int index] {
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		int IReadOnlyExtendedList<IMessage>.IndexOf(IMessage item) {
			throw new NotImplementedException();
		}

		public new void Commit() {
			throw new NotImplementedException();
		}

		public new void Rollback() {
			throw new NotImplementedException();
		}

		public new string Path { get; }
		public new Guid FileID { get; }
		public new TransactionalFileMappedBuffer AsBuffer { get; }

		public new void Dispose() {
			throw new NotImplementedException();
		}

		public void AddMessage(IMessage message) {
			throw new NotImplementedException();
		}

		public bool DeleteMessage(IMessage message) {
			throw new NotImplementedException();
		}

		public IMessage ReadMessage() {
			throw new NotImplementedException();
		}

		public IMessage RemoveMessage() {
			throw new NotImplementedException();
		}



		public ProcessingQueue(IItemSerializer<IMessage> serializer, string filename, string uncommittedPageFileDir, Guid fileID, int maxStorageBytes, int memoryCacheBytes, int maxItems, TransactionalFileMappedBuffer asBuffer, bool readOnly = false) : base(serializer, filename, uncommittedPageFileDir, fileID, maxStorageBytes, memoryCacheBytes, maxItems, readOnly) {
			AsBuffer = asBuffer;
		}

		public ProcessingQueue(IItemSerializer<IMessage> serializer, string filename, string uncommittedPageFileDir, Guid fileID, int transactionalPageSizeBytes, int maxStorageBytes, int memoryCacheBytes, int clusterSize, int maxItems, TransactionalFileMappedBuffer asBuffer, bool readOnly = false) : base(serializer, filename, uncommittedPageFileDir, fileID, transactionalPageSizeBytes, maxStorageBytes, memoryCacheBytes, clusterSize, maxItems, readOnly) {
			AsBuffer = asBuffer;
		}

		public ProcessingQueue(string filename, string uncommittedPageFileDir, Guid fileID, int memoryCacheBytes, IItemSerializer<IMessage> serializer, TransactionalFileMappedBuffer asBuffer, bool readOnly = false) : base(filename, uncommittedPageFileDir, fileID, memoryCacheBytes, serializer, readOnly) {
			AsBuffer = asBuffer;
		}

		public ProcessingQueue(string filename, string uncommittedPageFileDir, Guid fileID, int transactionalPageSizeBytes, int memoryCacheBytes, int clusterSize, IItemSerializer<IMessage> serializer, TransactionalFileMappedBuffer asBuffer, bool readOnly = false) : base(filename, uncommittedPageFileDir, fileID, transactionalPageSizeBytes, memoryCacheBytes, clusterSize, serializer, readOnly) {
			AsBuffer = asBuffer;
		}
	}
}
