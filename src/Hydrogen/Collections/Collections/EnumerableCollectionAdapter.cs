using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class EnumerableCollectionAdapter<TVal> : ReadOnlyCollectionBase<TVal> {
	private readonly Func<IEnumerator<TVal>> _enumeratorFunc;
	private readonly Func<TVal, bool> _containsFunc;
	private readonly Func<int> _countFunc;

	public EnumerableCollectionAdapter(Func<IEnumerator<TVal>> enumerator, Func<int> countFunc)
		: this(enumerator, countFunc, null) {
	}

	public EnumerableCollectionAdapter(Func<IEnumerator<TVal>> enumerator, Func<int> countFunc, Func<TVal, bool> containsFunc) {
		Guard.ArgumentNotNull(enumerator, nameof(enumerator));
		Guard.ArgumentNotNull(countFunc, nameof(countFunc));
		_enumeratorFunc = enumerator;
		_containsFunc = containsFunc ?? (x => _enumeratorFunc().AsEnumerable().Contains(x));
		_countFunc = countFunc;
	}

	public override int Count => _countFunc();

	public override IEnumerator<TVal> GetEnumerator() => _enumeratorFunc();

	public override bool Contains(TVal item) => _containsFunc(item);
}
