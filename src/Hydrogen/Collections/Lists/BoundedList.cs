// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class BoundedList<T, TInner> : ExtendedListDecorator<T, TInner>, IBoundedList<T> where TInner : IExtendedList<T> {

	public BoundedList(long startIndex, TInner listImpl)
		: base(listImpl) {
		FirstIndex = startIndex;
	}

	public long FirstIndex { get; }

	public override IEnumerable<long> IndexOfRange(IEnumerable<T> items) {
		var startIX = FirstIndex;
		return base.IndexOfRange(items).Select(x => x + startIX);
	}

	public override IEnumerable<T> ReadRange(long index, long count) {
		CheckRange(index, count);
		return base.ReadRange(index - FirstIndex, count);
	}

	public override void UpdateRange(long index, IEnumerable<T> items) {
		CheckRange(index, items.Count());
		base.UpdateRange(index - FirstIndex, items);
	}

	public override void InsertRange(long index, IEnumerable<T> items) {
		CheckRange(index, 0);
		base.InsertRange(index - FirstIndex, items);
	}

	public override void RemoveRange(long index, long count) {
		CheckRange(index, count);
		base.RemoveRange(index - FirstIndex, count);
	}

	protected void CheckRange(long index, long count) {
		var startIX = FirstIndex;
		var lastIX = startIX + (base.Count - 1).ClipTo(startIX, int.MaxValue);
		Guard.ArgumentInRange(index, startIX, lastIX, nameof(index));
		if (count > 0)
			Guard.ArgumentInRange(index + count - 1, startIX, lastIX, nameof(count));
	}

}

public class BoundedList<T> : BoundedList<T, IExtendedList<T>> {

	public BoundedList(long startIndex, IExtendedList<T> listImpl)
		: base(startIndex, listImpl) {
	}
}