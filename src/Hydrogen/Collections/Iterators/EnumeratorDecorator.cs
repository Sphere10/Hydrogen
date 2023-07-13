// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

public class EnumeratorDecorator<TFrom, TTo> : IEnumerator<TTo> where TTo : TFrom {

	public EnumeratorDecorator(IEnumerator<TFrom> enumerator) {
		InternalEumerator = enumerator;
	}

	protected readonly IEnumerator<TFrom> InternalEumerator;

	public virtual bool MoveNext() {
		return InternalEumerator.MoveNext();
	}

	public virtual void Reset() {
		InternalEumerator.Reset();
	}

	public virtual TTo Current => (TTo)InternalEumerator.Current;

	object IEnumerator.Current => Current;

	public virtual void Dispose() {
		InternalEumerator.Dispose();
	}

}


public class EnumeratorDecorator<T> : EnumeratorDecorator<T, T> {
	protected EnumeratorDecorator(IEnumerator<T> enumerator) : base(enumerator) {
	}
}
