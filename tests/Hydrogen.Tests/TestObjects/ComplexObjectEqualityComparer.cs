using System;
using System.Collections.Generic;

namespace Hydrogen.Tests;

public class ComplexObjectEqualityComparer : IEqualityComparer<ComplexObject> {
	private readonly ListEqualityComparer<ComplexObject> _listComparer;
	private readonly IEqualityComparer<object> _objectPropertyComparer;

	public ComplexObjectEqualityComparer(IEqualityComparer<object> objectPropertyComparer) {
		_listComparer = new ListEqualityComparer<ComplexObject>(this);
		_objectPropertyComparer = objectPropertyComparer;
	}

	public bool Equals(ComplexObject x, ComplexObject y) {
		if (ReferenceEquals(x, y))
			return true;
		if (ReferenceEquals(x, null))
			return false;
		if (ReferenceEquals(y, null))
			return false;
		if (x.GetType() != y.GetType())
			return false;
		return
			_objectPropertyComparer.Equals(x.ObjectProperty, y.ObjectProperty) &&
			Equals(x.RecursiveProperty, y.RecursiveProperty) &&
			TestObjectEqualityComparer.Instance.Equals(x.TestProperty, y.TestProperty) &&
			_listComparer.Equals(x.ManyRecursiveProperty, y.ManyRecursiveProperty);
	}
	public int GetHashCode(ComplexObject obj) {
		return HashCode.Combine(obj.ObjectProperty, obj.RecursiveProperty, obj.TestProperty, obj.ManyRecursiveProperty);
	}
}