using System;
using System.Collections.Generic;

namespace Hydrogen;

public class PackedEqualityComparer : IEqualityComparer<object> {
	private readonly object _comparer;
	private readonly IEqualityComparer<object> _projectedComparer;
	
	private PackedEqualityComparer(object comparer, IEqualityComparer<object> projectedComparer) {
		Guard.ArgumentNotNull(comparer, nameof(comparer));
		Guard.ArgumentNotNull(projectedComparer, nameof(projectedComparer));
		Guard.Argument(comparer.GetType().IsSubtypeOfGenericType(typeof(IEqualityComparer<>)), nameof(projectedComparer), "Must be an IComparer<>");	
		Guard.Argument(projectedComparer.GetType().IsSubtypeOfGenericType(typeof(ProjectedEqualityComparer<,>)), nameof(projectedComparer), "Must be an ProjectedEqualityComparer<>");	
		_comparer = comparer;
		_projectedComparer = projectedComparer;
	}

	public bool Equals(object x, object y) => _projectedComparer.Equals(x, y);

	public int GetHashCode(object obj) => _projectedComparer.GetHashCode(obj);

	public static PackedEqualityComparer Pack<TItem>(IEqualityComparer<TItem> comparer) {
		Guard.ArgumentNotNull(comparer, nameof(comparer));
		return new PackedEqualityComparer(comparer, comparer.AsProjection(x => (object)x, x => (TItem)x));
	}

	public IEqualityComparer<TItem> Unpack<TItem>() {
		var unpacked = _comparer as IEqualityComparer<TItem>;
		Guard.Ensure(unpacked != null, $"Cannot unpack {_comparer.GetType().Name} as is not an IEqualityComparer<{typeof(TItem).Name}>");
		return unpacked;
	}

}
