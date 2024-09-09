using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace Hydrogen.Maths;

public class ErrorBandEqualityComparer : IEqualityComparer<decimal> {
	private readonly decimal _tolerance;

	public ErrorBandEqualityComparer(decimal tolerance) {
		if (tolerance < 0) {
			throw new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance must be non-negative.");
		}
		_tolerance = tolerance;
	}

	public bool Equals(decimal x, decimal y) {
		return Math.Abs(x - y) <= _tolerance;
	}

	public int GetHashCode(decimal obj) => throw new NotSupportedException("Reason: loss of equivalence under transitivity due to the inexact nature of the comparison");
}