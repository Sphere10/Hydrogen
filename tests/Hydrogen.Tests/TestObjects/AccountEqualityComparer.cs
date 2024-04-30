using System;
using System.Collections.Generic;

namespace Hydrogen.Tests;

public class AccountEqualityComparer : IEqualityComparer<Account> {
	public bool Equals(Account x, Account y) {
		if (ReferenceEquals(x, y))
			return true;
		if (ReferenceEquals(x, null))
			return false;
		if (ReferenceEquals(y, null))
			return false;
		if (x.GetType() != y.GetType())
			return false;
		return x.Name == y.Name && x.Quantity == y.Quantity && x.UniqueNumber == y.UniqueNumber && Equals(x.Identity, y.Identity);
	}
	public int GetHashCode(Account obj) {
		return HashCode.Combine(obj.Name, obj.Quantity, obj.UniqueNumber, obj.Identity);
	}
}
