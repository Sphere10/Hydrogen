using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// A base class for batch-optimized extended lists. Much of the boiler plate code is provided, implementations only
	/// care about important methods.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class RangedListBase<T> : ExtendedListBase<T> {

		protected volatile int Version;

		protected RangedListBase() {
			Version = 0;
		}

		public override bool IsReadOnly => false;

		public sealed override int IndexOf(T item) {
			return IndexOfRange(new[] { item }).Single();
		}

		public sealed override bool Contains(T item) {
			return ContainsRange(new[] { item }).First();
		}

		public override IEnumerable<bool> ContainsRange(IEnumerable<T> items) => IndexOfRange(items).Select(ix => ix >= 0);

		public sealed override T Read(int index) {
			return ReadRange(index, 1).Single();
		}

		public sealed override void Add(T item) {
			AddRange(new[] { item });
		}

		public sealed override void Update(int index, T item) {
			UpdateRange(index, new[] { item });
		}

		public sealed override void Insert(int index, T item) {
			InsertRange(index, new[] { item });
		}

		public sealed override void RemoveAt(int index) {
			RemoveRange(index, 1);
		}

		public sealed override bool Remove(T item) {
			return RemoveRange(new[] { item }).First();
		}

		public override IEnumerable<bool> RemoveRange(IEnumerable<T> items) {
			var itemsArr = items as T[] ?? items.ToArray();

			// Remove nothing
			if (itemsArr.Length == 0)
				return Enumerable.Empty<bool>();

			// optimize for single read
			if (itemsArr.Length == 1) {
				var ix = IndexOfRange(itemsArr).First();
				if (ix >= 0) {
					RemoveRange(ix, 1);
					return new[] { true };
				}
				return new[] { false };
			}
			throw new NotImplementedException("For more than 1 item, todo");
			//var itemIndexes =  itemsArr.ZipWith(IndexOfRange(itemsArr), Tuple.Create);

			//// remove in reverse
			//itemIndexes.OrderByDescending(x => x.Item2).RemoveRange(
		}

		public override void Clear() {
			RemoveRange(0, Count);
		}

		public override void CopyTo(T[] array, int arrayIndex) {
			foreach (var item in this)
				array[arrayIndex++] = item;
		}

		public override IEnumerator<T> GetEnumerator() {
			var version = this.Version;
			for (var i = 0; i < Count; i++) {
				CheckVersion(version);
				yield return Read(i);
			}
		}

		protected void UpdateVersion() {
			unchecked {
				Version++;
			}
		}

		protected void CheckNotReadonly() {
			if (IsReadOnly)
				throw new InvalidOperationException("Collection is read-only");
		}

		protected void CheckVersion(int enumeratedVersion) {
			if (Version != enumeratedVersion) {
				throw new InvalidOperationException("Collection was changed during enumeration");
			}
		}

	}

}