// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public sealed class ProjectedLookup<TKey, TValue, TProjectedKey, TProjectedValue> : ILookup<TProjectedKey, TProjectedValue> {
	private readonly ILookup<TKey, TValue> _source;
	private readonly Func<TKey, TProjectedKey> _keyProjection;
	private readonly Func<TProjectedKey, TKey> _inverseKeyProjection;
	private readonly Func<TValue, TProjectedValue> _valueProjection;

	public ProjectedLookup(ILookup<TKey, TValue> source, Func<TKey, TProjectedKey> keyProjection, Func<TProjectedKey, TKey> inverseKeyProjection, Func<TValue, TProjectedValue> valueProjection) {
		_source = source;
		_keyProjection = keyProjection;
		_inverseKeyProjection = inverseKeyProjection;
		_valueProjection = valueProjection;
	}

	public int Count => _source.Count;

	public IEnumerable<TProjectedValue> this[TProjectedKey key] => throw new NotImplementedException();

	public bool Contains(TProjectedKey key) => _source.Contains(_inverseKeyProjection(key));

	public IEnumerator<IGrouping<TProjectedKey, TProjectedValue>> GetEnumerator() 
		=> _source.Select(x => new Grouping<TProjectedKey, TProjectedValue>(_keyProjection(x.Key), x.Select(_valueProjection))).GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	
}
