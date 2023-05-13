// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Hydrogen {

	public class BoundedList<T> : ExtendedListDecorator<T>, IBoundedList<T> {

		public BoundedList(int startIndex, IExtendedList<T> listImpl)
			: base(listImpl) {
			FirstIndex = startIndex;
		}

		public int FirstIndex { get; }

		public override IEnumerable<int> IndexOfRange(IEnumerable<T> items) {
			var startIX = FirstIndex;
			return base.IndexOfRange(items).Select(x => x + startIX);
		}

		public override IEnumerable<T> ReadRange(int index, int count) {
			CheckRange(index, count);
			return base.ReadRange(index - FirstIndex, count);
		}

		public override void UpdateRange(int index, IEnumerable<T> items) {
			CheckRange(index, items.Count());
			base.UpdateRange(index - FirstIndex, items);
		}

		public override void InsertRange(int index, IEnumerable<T> items) {
			CheckRange(index, 0);
			base.InsertRange(index - FirstIndex, items);
		}

		public override void RemoveRange(int index, int count) {
			CheckRange(index, count);
			base.RemoveRange(index - FirstIndex, count);
		}

		protected void CheckRange(int index, int count) {
			var startIX = FirstIndex;
			var lastIX = startIX + (base.Count - 1).ClipTo(startIX, int.MaxValue);
			Guard.ArgumentInRange(index, startIX, lastIX, nameof(index));
			if (count > 0)
				Guard.ArgumentInRange(index + count - 1, startIX, lastIX, nameof(count));
		}

	}

}
