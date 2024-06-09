// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class LookupDecorator<TKey, TValue, TInner> : ILookup<TKey, TValue> where TInner : ILookup<TKey, TValue> {
	internal readonly ILookup<TKey, TValue> InternalLookup;

	public LookupDecorator(TInner internalLookup) {
		Guard.ArgumentNotNull(internalLookup, nameof(internalLookup));
		InternalLookup = internalLookup;
	}

	public virtual int Count => InternalLookup.Count;

	public virtual bool Contains(TKey key) => InternalLookup.Contains(key);

	public IEnumerable<TValue> this[TKey key] => InternalLookup[key];

	public virtual IEnumerator<IGrouping<TKey, TValue>> GetEnumerator() => InternalLookup.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}

public class LookupDecorator<TKey, TValue> : LookupDecorator<TKey, TValue, ILookup<TKey, TValue>> {
	public LookupDecorator(ILookup<TKey, TValue> internalLookup) : base(internalLookup) {
	}
}