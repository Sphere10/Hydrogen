using System;
using System.Collections.Generic;

namespace Hydrogen;

public class ProjectedEqualityComparer<TFrom, TTo> : IEqualityComparer<TTo> {

	private readonly IEqualityComparer<TFrom> _sourceComparer;
	private readonly Func<TTo, TFrom> _inverseProjection;

	public ProjectedEqualityComparer(IEqualityComparer<TFrom> sourceComparer, Func<TTo, TFrom> inverseProjection) {
		_sourceComparer = sourceComparer;
		_inverseProjection = inverseProjection;
	}

	public bool Equals(TTo x, TTo y) {
		if (x == null && y == null) {
			return true;
		}
		if (x == null || y == null) {
			return false;
		}
		return _sourceComparer.Equals(_inverseProjection(x), _inverseProjection(y));
	}

	public int GetHashCode(TTo obj) =>  _sourceComparer.GetHashCode(_inverseProjection(obj));
}
