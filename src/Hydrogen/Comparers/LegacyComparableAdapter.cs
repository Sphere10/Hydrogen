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

public class LegacyComparableAdapter<T> : IComparable<T>, IComparable, IEquatable<T> {
	public readonly T @Object;
	private readonly Comparer<T> _comparer;

	public LegacyComparableAdapter(T internalObject) {
		@Object = internalObject;
		_comparer = Comparer<T>.Default;
	}


	public int CompareTo(T other) {
		return _comparer.Compare(@Object, other);
	}

	public int CompareTo(object obj) {
		return Comparer<object>.Default.Compare(@Object, obj);
	}

	public bool Equals(T other) {
		return CompareTo(other) == 0;
	}
}
