using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class ListEqualityComparer<T> : IEqualityComparer<IList<T>> {
	private readonly IEqualityComparer<T> _itemComparer;

	public ListEqualityComparer() : this(EqualityComparer<T>.Default) { }

	public ListEqualityComparer(IEqualityComparer<T> itemComparer) {
		_itemComparer = itemComparer ?? EqualityComparer<T>.Default;
	}

	public bool Equals(IList<T> x, IList<T> y) {
		if (ReferenceEquals(x, y)) return true;
		if (x == null || y == null) return false;
		if (x.Count != y.Count) return false;
		return x.SequenceEqual(y, _itemComparer);
	}

	public int GetHashCode(IList<T> obj) 
		=> obj.Select(x => HashCode.Combine(x)).Aggregate(HashCode.Combine(obj), HashCode.Combine);

}
