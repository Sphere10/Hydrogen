using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
public class HashSetTests : SetTestsBase {
	protected override IDisposable CreateSet<TValue>(IEqualityComparer<TValue> comparer, out ISet<TValue> set) {
		set = new HashSet<TValue>(comparer);
		return Disposables.None;
	}
}
