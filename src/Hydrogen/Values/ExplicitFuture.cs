// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// A future whose value will be explicitly set by the client code. If the value is requested before being set, an exception is thrown.
/// If the value is attempted to be set after already been set, an exception is thrown. 
/// </summary>
public class ExplicitFuture<T> : IFuture<T> {
	T _value;
	bool _valueSet;

	/// <summary>
	/// Returns the value of the future, once it has been set
	/// </summary>
	/// <exception cref="InvalidOperationException">If the value is not yet available</exception>
	public T Value {
		get {
			Guard.Ensure(_valueSet, "No value has been set yet");
			return _value;
		}
		set {
			Guard.Against(_valueSet, "Value has already been set");
			_valueSet = true;
			this._value = value;
		}
	}


	public static ExplicitFuture<T> For(T value) {
		return new ExplicitFuture<T> { Value = value };
	}

	/// <summary>
	/// Returns a string representation of the value if available, null otherwise
	/// </summary>
	/// <returns>A string representation of the value if available, null otherwise</returns>
	public override string ToString() {
		return _valueSet ? Convert.ToString(_value) : "Future value has not currently been determined";
	}
}
