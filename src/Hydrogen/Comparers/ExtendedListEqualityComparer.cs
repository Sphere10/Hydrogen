using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class ExtendedListEqualityComparer<T> : IEqualityComparer<IExtendedList<T>> {
	private readonly IEqualityComparer<T> _itemComparer;

	public ExtendedListEqualityComparer() : this(EqualityComparer<T>.Default) { }

	public ExtendedListEqualityComparer(IEqualityComparer<T> itemComparer) {
		_itemComparer = itemComparer ?? EqualityComparer<T>.Default;
	}

	public bool Equals(IExtendedList<T> x, IExtendedList<T> y) {
		if (ReferenceEquals(x, y)) return true;
		if (x == null || y == null) return false;
		if (x.Count != y.Count) return false;
		return x.SequenceEqual(y, _itemComparer);
	}

	public int GetHashCode(IExtendedList<T> obj) 
		=> obj.Select(x => HashCode.Combine(x)).Aggregate(HashCode.Combine(obj), HashCode.Combine);

}
