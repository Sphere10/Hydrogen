using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Sphere10.Framework;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Queue {
	public class PrivateQueue : TransactionalList<IMessage>, IHeliumQueue {
		private readonly int _count;
		private readonly int _count1;
		private readonly int _count2;

		//public PrivateQueue(int count, int count1, int count2) {
		//	_count = count;
		//	_count1 = count1;
		//	_count2 = count2;
		//}

		public PrivateQueue(PrivateQueueConfigDto privateQueueConfigDto)
			: base(
				new BinaryFormattedSerializer<IMessage>(),
				privateQueueConfigDto.Path,
				privateQueueConfigDto.TempDirPath,
				privateQueueConfigDto.FileId,
				privateQueueConfigDto.TransactionalPageSizeBytes,
				privateQueueConfigDto.MaxStorageSizeBytes,
				privateQueueConfigDto.AllocatedMemory,
				privateQueueConfigDto.ClusterSize,
				privateQueueConfigDto.MaxItems
			) {
		}

		public IEnumerator<IMessage> GetEnumerator() {
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

		public void AddRange(IEnumerable<IMessage> items) {
			throw new NotImplementedException();
		}

		bool IWriteOnlyExtendedCollection<IMessage>.Remove(IMessage item) {
			throw new NotImplementedException();
		}

		int IExtendedCollection<IMessage>.Count => _count2;

		public IEnumerable<bool> RemoveRange(IEnumerable<IMessage> items) {
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

		public IEnumerable<bool> ContainsRange(IEnumerable<IMessage> items) {
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

		public bool IsReadOnly { get; }

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

		public IEnumerable<int> IndexOfRange(IEnumerable<IMessage> items) {
			throw new NotImplementedException();
		}

		public IMessage Read(int index) {
			throw new NotImplementedException();
		}

		public IEnumerable<IMessage> ReadRange(int index, int count) {
			throw new NotImplementedException();
		}

		public void Update(int index, IMessage item) {
			throw new NotImplementedException();
		}

		public void UpdateRange(int index, IEnumerable<IMessage> items) {
			throw new NotImplementedException();
		}

		void IWriteOnlyExtendedList<IMessage>.Insert(int index, IMessage item) {
			throw new NotImplementedException();
		}

		public void InsertRange(int index, IEnumerable<IMessage> items) {
			throw new NotImplementedException();
		}

		void IWriteOnlyExtendedList<IMessage>.RemoveAt(int index) {
			throw new NotImplementedException();
		}

		public void RemoveRange(int index, int count) {
			throw new NotImplementedException();
		}

		void IList<IMessage>.Insert(int index, IMessage item) {
			throw new NotImplementedException();
		}

		void IList<IMessage>.RemoveAt(int index) {
			throw new NotImplementedException();
		}

		public IMessage this[int index] {
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		int IReadOnlyExtendedList<IMessage>.IndexOf(IMessage item) {
			throw new NotImplementedException();
		}

		public event EventHandlerEx<object> Committing;
		public event EventHandlerEx<object> Committed;
		public event EventHandlerEx<object> RollingBack;
		public event EventHandlerEx<object> RolledBack;
		
		public void Commit() {
			throw new NotImplementedException();
		}

		public void Rollback() {
			throw new NotImplementedException();
		}

		public string Path { get; }
		public Guid FileID { get; }
		public TransactionalFileMappedBuffer AsBuffer { get; }
		public bool RequiresLoad { get; }

		public void Load() {
			throw new NotImplementedException();
		}

		public ReaderWriterLockSlim ThreadLock { get; }

		public Scope EnterReadScope() {
			throw new NotImplementedException();
		}

		public Scope EnterWriteScope() {
			throw new NotImplementedException();
		}

		public void Dispose() {
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

		//TODO: Jake REMEMBER to remove this//
		private void ThisIsToRemoveTheNotUsedWarning() {
			Debug.Assert(Committing != null, nameof(Committing) + " != null");
			var x = Committing.ToString();

			Debug.Assert(Committing != null, nameof(Committing) + " != null");
			var y = Committed.ToString();

			Debug.Assert(Committing != null, nameof(Committing) + " != null");
			var z = RollingBack.ToString();

			Debug.Assert(Committing != null, nameof(Committing) + " != null");
			var a = RolledBack.ToString();
		}
	}
}
