// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public sealed class Synchronized<T> : SynchronizedObject {
	private T _value;

	public Synchronized(T @object) {
		_value = @object;
	}

	public T Value {
		get {
			using (EnterReadScope())
				return _value;
		}
		set {
			using (EnterWriteScope())
				_value = value;
		}
	}
}
