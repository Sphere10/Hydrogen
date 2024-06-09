// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

sealed class ClassNullOp<T> : INullOp<T>
	where T : class {
	public bool HasValue(T value) {
		return value != null;
	}
	public bool AddIfNotNull(ref T accumulator, T value) {
		if (value != null) {
			accumulator = accumulator == null ? value : Operator<T>.Add(accumulator, value);
			return true;
		}
		return false;
	}
}
