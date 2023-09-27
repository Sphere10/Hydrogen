using System;
using System.Collections.Generic;

namespace Hydrogen;

public class ProjectedComparer<TFrom, TTo> : IComparer<TTo> {
	private readonly IComparer<TFrom> _sourceComparer;
	private readonly Func<TTo, TFrom> _inverseProjection;

	public ProjectedComparer(IComparer<TFrom> sourceComparer, Func<TTo, TFrom> inverseProjection) {
		_sourceComparer = sourceComparer;
		_inverseProjection = inverseProjection;
	}

	public int Compare(TTo x, TTo y) {
		if (x == null && y == null) {
			return 0;
		}

		if (x == null) {
			return -1;
		}

		if (y == null) {
			return 1;
		}
		return _sourceComparer.Compare(_inverseProjection(x), _inverseProjection(y));
	}
}