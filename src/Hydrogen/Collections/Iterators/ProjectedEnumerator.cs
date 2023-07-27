using System;
using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

public class ProjectedEnumerator<TFrom, TTo> : IEnumerator<TTo>  {
	private readonly Func<TFrom, TTo> _projection;

	public ProjectedEnumerator(IEnumerator<TFrom> enumerator, Func<TFrom, TTo> projection) {
		InternalEnumerator = enumerator;
		_projection = projection;
	}

	protected readonly IEnumerator<TFrom> InternalEnumerator;

	public virtual bool MoveNext() {
		return InternalEnumerator.MoveNext();
	}

	public virtual void Reset() {
		InternalEnumerator.Reset();
	}

	public virtual TTo Current => _projection(InternalEnumerator.Current);

	object IEnumerator.Current => Current;

	public virtual void Dispose() {
		InternalEnumerator.Dispose();
	}

}
