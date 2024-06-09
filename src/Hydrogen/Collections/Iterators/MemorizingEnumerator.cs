// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

#warning Untested


/// <summary>
/// Memorizes values fetched on first iteration and produces memorized result on subsequent iterations. Useful for
/// wrapping an 
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class MemorizingEnumerator<T> : EnumeratorDecorator<T> {
	private readonly IList<T> _memory;
	private int _index;
	private bool _memorized;

	public MemorizingEnumerator(IEnumerator<T> enumerator)
		: this(enumerator, new List<T>()) {
	}

	public MemorizingEnumerator(IEnumerator<T> enumerator, IList<T> memory)
		: base(enumerator) {
		_memory = memory;
		_index = -1;
		_memorized = false;
	}

	public override bool MoveNext() {
		var nextIndex = _index + 1;
		if (0 <= nextIndex && nextIndex < _memory.Count) {
			return true;
		}

		if (base.MoveNext()) {
			_memory.Add(base.Current);
			_index++;
			return true;
		}
		_memorized = true;
		return false;
	}

	public override void Reset() {
		base.Reset();
		_index = -1;
	}

	public override T Current {
		get {
			if (0 <= _index && _index < _memory.Count) {
				return _memory[_index];
			}
			throw new InternalErrorException();
		}
	}

	internal int MemorizedCount => _memorized ? _memory.Count : throw new InvalidOperationException();
}
