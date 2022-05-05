using System.Collections.Generic;

namespace Hydrogen {

	public abstract class ExtendedListBase<T> : ExtendedCollectionBase<T>, IExtendedList<T> {

		public T this[int index] { get => Read(index); set => Update(index, value); }

		public abstract int IndexOf(T item);

		public abstract IEnumerable<int> IndexOfRange(IEnumerable<T> items);

		public abstract T Read(int index);

		public abstract IEnumerable<T> ReadRange(int index, int count);

		public abstract void Update(int index, T item);

		public abstract void UpdateRange(int index, IEnumerable<T> items);

		public abstract void Insert(int index, T item);

		public abstract void InsertRange(int index, IEnumerable<T> items);

		public abstract void RemoveAt(int index);

		public abstract void RemoveRange(int index, int count);

		protected virtual void CheckIndex(int index, bool allowAtEnd = false) => Guard.CheckIndex(index, 0, Count, allowAtEnd);

		protected virtual void CheckRange(int index, int count, bool rightAligned = false) => Guard.CheckRange(index, count, rightAligned, 0, Count);

	}

}