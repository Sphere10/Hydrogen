using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sphere10.Framework {

	public class BoundedList<T> : ExtendedListDecorator<T>, IBoundedList<T> {

		public BoundedList(int startIndex, IExtendedList<T> listImpl)
			: base(listImpl) {
			StartIndex = startIndex;
		}

		public int StartIndex { get; }

		public override IEnumerable<int> IndexOfRange(IEnumerable<T> items) {
			var startIX = StartIndex;
			return base.IndexOfRange(items).Select(x => x + startIX);
		}

		public override IEnumerable<T> ReadRange(int index, int count) {
			CheckRange(index, count);
			return base.ReadRange(index - StartIndex, count);
		}

		public override void UpdateRange(int index, IEnumerable<T> items) {
			CheckRange(index, items.Count());
			base.UpdateRange(index - StartIndex, items);
		}

		public override void InsertRange(int index, IEnumerable<T> items) {
			CheckRange(index, 0);
			base.InsertRange(index - StartIndex, items);
		}

		public override void RemoveRange(int index, int count) {
			CheckRange(index, count);
			base.RemoveRange(index - StartIndex, count);
		}

		protected void CheckRange(int index, int count) {
			var startIX = StartIndex;
			var lastIX = startIX + (base.Count - 1).ClipTo(startIX, int.MaxValue);
			Guard.ArgumentInRange(index, startIX, lastIX, nameof(index));
			if (count > 0)
				Guard.ArgumentInRange(index + count - 1, startIX, lastIX, nameof(count));
		}

	}

}
