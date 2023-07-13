// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
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
