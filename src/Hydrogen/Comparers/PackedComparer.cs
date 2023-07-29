using System.Collections.Generic;

namespace Hydrogen;

public class PackedComparer : IComparer<object> {
	private readonly object _comparer;
	private readonly IComparer<object> _projectedComparer;
	
	private PackedComparer(object comparer, IComparer<object> projectedComparer) {
		Guard.ArgumentNotNull(comparer, nameof(comparer));
		Guard.ArgumentNotNull(projectedComparer, nameof(projectedComparer));
		Guard.Argument(comparer.GetType().IsSubtypeOfGenericType(typeof(IComparer<>)), nameof(projectedComparer), "Must be an IComparer<>");	
		Guard.Argument(projectedComparer.GetType().IsSubtypeOfGenericType(typeof(ProjectedComparer<,>)), nameof(projectedComparer), "Must be an ProjectedComparer<>");	
		_comparer = comparer;
		_projectedComparer = projectedComparer;
	}

	public int Compare(object x, object y) => _projectedComparer.Compare(x, y);

	public static PackedComparer Pack<TItem>(IComparer<TItem> comparer) {
		Guard.ArgumentNotNull(comparer, nameof(comparer));
		return new PackedComparer(comparer, comparer.AsProjection(x => (object)x, x => (TItem)x));
	}

	public IComparer<TItem> Unpack<TItem>() {
		var unpacked = _comparer as IComparer<TItem>;
		Guard.Ensure(unpacked != null, $"Cannot unpack {_comparer.GetType().Name} as is not an IComparer<{typeof(TItem).Name}>");
		return unpacked;
	}
	
}
