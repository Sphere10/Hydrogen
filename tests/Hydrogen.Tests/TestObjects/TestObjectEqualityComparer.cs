using System;
using System.Collections.Generic;

namespace Hydrogen.Tests;

public class TestObjectEqualityComparer : IEqualityComparer<TestObject> {

	public static TestObjectEqualityComparer Instance { get; } = new TestObjectEqualityComparer();

	public bool Equals(TestObject x, TestObject y) {
		if (ReferenceEquals(x, y))
			return true;
		if (ReferenceEquals(x, null))
			return false;
		if (ReferenceEquals(y, null))
			return false;
		if (x.GetType() != y.GetType())
			return false;
		return x.A == y.A && x.B == y.B && x.C == y.C;
	}
	public int GetHashCode(TestObject obj) {
		return HashCode.Combine(obj.A, obj.B, obj.C);
	}
}
