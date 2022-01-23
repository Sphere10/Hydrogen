using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// Base class for singular item-by-item based extended list implementations. This is not optimized for batch access.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class SingularListBase<T> : ExtendedListBase<T> {

		public override bool IsReadOnly => false;

		public override IEnumerable<bool> ContainsRange(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			return items.Select(Contains).ToArray();
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			return items.Select(IndexOf);
		}

		public override IEnumerable<T> ReadRange(int index, int count) {
			Guard.ArgumentGTE(count, 0, nameof(index));
			return Enumerable.Range(index, count).Select(Read);
		}

		public override void UpdateRange(int index, IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			items.ForEach(x => Update(index++, x));
		}

		public override void InsertRange(int index, IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			items.ForEach(x => Insert(index++, x));
		}

		public override void RemoveRange(int index, int count) {
			Guard.ArgumentGTE(count, 0, nameof(index));
			Tools.Collection.Repeat(() => RemoveAt(index), count);
		}

		public override void AddRange(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			items.ForEach(Add);
		}

		public override IEnumerable<bool> RemoveRange(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			foreach (var item in items)
				yield return Remove(item);
		}

		protected int EnsureSafe(int index, bool allowAtEnd = false) {
			CheckIndex(index, allowAtEnd);
			return index;
		}

		protected IEnumerable<T> EnsureSafe(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			return items;
		}
	}

}