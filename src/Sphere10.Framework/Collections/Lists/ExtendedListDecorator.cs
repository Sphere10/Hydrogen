using System.Collections;
using System.Collections.Generic;

namespace Sphere10.Framework {

	/// <summary>
	/// Decorator for an IExtendedList, but calls to non-range get routed to the range-based methods.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class ExtendedListDecorator<TItem> : IExtendedList<TItem> {

		protected ExtendedListDecorator(IExtendedList<TItem> internalExtendedList) {
			Guard.ArgumentNotNull(internalExtendedList, nameof(internalExtendedList));
			InternalExtendedList = internalExtendedList;
		}

		protected IExtendedList<TItem> InternalExtendedList { get; }

		public virtual int Count => InternalExtendedList.Count;

		public virtual bool IsReadOnly => InternalExtendedList.IsReadOnly;

        public virtual int IndexOf(TItem item) => InternalExtendedList.IndexOf(item);
		
		public virtual IEnumerable<int> IndexOfRange(IEnumerable<TItem> items) => InternalExtendedList.IndexOfRange(items);

		public virtual bool Contains(TItem item) => InternalExtendedList.Contains(item);

		public virtual IEnumerable<bool> ContainsRange(IEnumerable<TItem> items) => InternalExtendedList.ContainsRange(items);

		public virtual TItem Read(int index) => InternalExtendedList.Read(index);

		public virtual IEnumerable<TItem> ReadRange(int index, int count) => InternalExtendedList.ReadRange(index, count);

		public virtual void Add(TItem item) => InternalExtendedList.Add(item);

		public virtual void AddRange(IEnumerable<TItem> items) => InternalExtendedList.AddRange(items);
		
		public virtual void Update(int index, TItem item) => InternalExtendedList.Update(index, item);

		public virtual void UpdateRange(int index, IEnumerable<TItem> items) =>	InternalExtendedList.UpdateRange(index, items);

		public virtual void Insert(int index, TItem item) => InternalExtendedList.Insert(index, item);
		
		public virtual void InsertRange(int index, IEnumerable<TItem> items) => InternalExtendedList.InsertRange(index, items);

		public virtual bool Remove(TItem item) => InternalExtendedList.Remove(item);

		public virtual IEnumerable<bool> RemoveRange(IEnumerable<TItem> items) => InternalExtendedList.RemoveRange(items);

		public virtual void RemoveAt(int index) =>	InternalExtendedList.RemoveAt(index);

		public virtual void RemoveRange(int index, int count) => InternalExtendedList.RemoveRange(index, count);

		public virtual void Clear() => InternalExtendedList.Clear();

		public virtual void CopyTo(TItem[] array, int arrayIndex) => InternalExtendedList.CopyTo(array, arrayIndex);

		public virtual IEnumerator<TItem> GetEnumerator() => InternalExtendedList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public TItem this[int index] { get => this.Read(index); set => this.Update(index, value); }

		TItem IWriteOnlyExtendedList<TItem>.this[int index] { set => this[index] = value; }

		TItem IReadOnlyList<TItem>.this[int index] => this[index];

	}

}

