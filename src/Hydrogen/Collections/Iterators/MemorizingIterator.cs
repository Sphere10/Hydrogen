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

namespace Hydrogen;

#warning Untested


/// <summary>
/// Memorizes the enumerable and resolves the "multiple enumeration problem".
/// </summary>
/// <typeparam name="T"></typeparam>
public struct MemorizingIterator<T> : IEnumerable<T>, ICollection<T> {
	private readonly IEnumerable<T> _source;
	private MemorizingEnumerator<T> _memorizingEnumerator;

	public MemorizingIterator(IEnumerable<T> enumerable) {
		_source = enumerable;
		_memorizingEnumerator = null;
	}

	public IEnumerator<T> GetEnumerator() {
		_memorizingEnumerator ??= new MemorizingEnumerator<T>(_source.GetEnumerator());
		return _memorizingEnumerator;
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	public int Count {
		get {
			Guard.ArgumentNotNull(_source, nameof(_source));
			switch (_source) {
				case ICollection<T> c:
					return c.Count;
				case ICollection collection:
					return collection.Count;
				case IReadOnlyCollection<T> roc:
					return roc.Count;
			}
			if (_memorizingEnumerator != null) {
				return _memorizingEnumerator.MemorizedCount;
			}
			using var e = GetEnumerator();
			while (e.MoveNext()) {
				// memorizes
			}
			// ReSharper disable once PossibleNullReferenceException
			return _memorizingEnumerator.MemorizedCount;
		}
	}

	#region ICollection Implementation (Special)

	// Note, this is required due to https://github.com/dotnet/runtime/issues/43001 

	public void Add(T item) {
		throw new NotImplementedException();
	}

	public void Clear() {
		throw new NotImplementedException();
	}

	public bool Contains(T item) {
		throw new NotImplementedException();
	}

	public void CopyTo(T[] array, int arrayIndex) {
		throw new NotImplementedException();
	}

	public bool Remove(T item) {
		throw new NotImplementedException();
	}

	public bool IsReadOnly => throw new NotImplementedException();

	#endregion

}
