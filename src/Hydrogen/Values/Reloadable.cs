// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// A future whose value is fetched lazily and which can be invalidated by client. By invalidating the value, the future re-loads the value on the subsequent request.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Reloadable<T> : SynchronizedObject, IFuture<T> {
	private bool _loaded;
	private T _value;
	private readonly Func<T> _loader;

	public Reloadable(Func<T> valueLoader) {
		_loader = valueLoader;
		_loaded = false;
		_value = default;
	}

	public T Value {
		get {
			if (Loaded)
				return _value;
			using (EnterWriteScope()) {
				if (Loaded)
					return _value;

				_value = _loader();
				_loaded = true;
				return _value;
			}
		}
	}

	public bool Loaded {
		get {
			using (EnterReadScope()) {
				return _loaded;
			}
		}
	}

	public void Invalidate() {
		using (EnterWriteScope()) {
			_loaded = false;
		}
	}

	public static Reloadable<T> From(Func<T> valueLoader) {
		return new Reloadable<T>(valueLoader);
	}

	public override string ToString() {
		using (EnterReadScope()) {
			return _loaded ? Convert.ToString(_value) : "Future value has not currently been determined";
		}
	}
}
