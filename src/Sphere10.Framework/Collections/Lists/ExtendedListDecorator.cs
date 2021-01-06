using System.Collections;
using System.Collections.Generic;

namespace Sphere10.Framework {

	/// <summary>
	/// Decorator for an IExtendedList, but calls to non-range get routed to the range-based methods.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class ExtendedListDecorator<TItem> : ExtendedListBase<TItem> {

		protected ExtendedListDecorator(IExtendedList<TItem> internalExtendedList) {
			InternalExtendedList = internalExtendedList;
		}

		protected IExtendedList<TItem> InternalExtendedList { get; }

		public override int Count => InternalExtendedList.Count;

		public override bool IsReadOnly => InternalExtendedList.IsReadOnly;

		public override int IndexOf(TItem item) {
			return InternalExtendedList.IndexOf(item);
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<TItem> items) {
			return InternalExtendedList.IndexOfRange(items);
		}

		public override bool Contains(TItem item) {
			return InternalExtendedList.Contains(item);
		}

		public override IEnumerable<bool> ContainsRange(IEnumerable<TItem> items) {
			return InternalExtendedList.ContainsRange(items);
		}

		public override TItem Read(int index) {
			return InternalExtendedList.Read(index);
		}

		public override IEnumerable<TItem> ReadRange(int index, int count) {
			return InternalExtendedList.ReadRange(index, count);
		}

		public override void Add(TItem item) {
			InternalExtendedList.Add(item);
		}

		public override void AddRange(IEnumerable<TItem> items) {
			InternalExtendedList.AddRange(items);
		}
		
		public override void Update(int index, TItem item) {
			InternalExtendedList.Update(index, item);
		}

		public override void UpdateRange(int index, IEnumerable<TItem> items) {
			InternalExtendedList.UpdateRange(index, items);
		}

		public override void Insert(int index, TItem item) {
			InternalExtendedList.Insert(index, item);
		}

		public override void InsertRange(int index, IEnumerable<TItem> items) {
			InternalExtendedList.InsertRange(index, items);
		}

		public override bool Remove(TItem item) {
			return InternalExtendedList.Remove(item);
		}

		public override IEnumerable<bool> RemoveRange(IEnumerable<TItem> items) {
			return InternalExtendedList.RemoveRange(items);
		}

		public override void RemoveAt(int index) {
			InternalExtendedList.RemoveAt(index);
		}

		public override void RemoveRange(int index, int count) {
			InternalExtendedList.RemoveRange(index, count);
		}

		public override void Clear() {
			InternalExtendedList.Clear();
		}

		public override void CopyTo(TItem[] array, int arrayIndex) {
			InternalExtendedList.CopyTo(array, arrayIndex);
		}

		public override IEnumerator<TItem> GetEnumerator() {
			return InternalExtendedList.GetEnumerator();
		}

	}

}

