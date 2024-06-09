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
/// A future whose value is fetched on first request and retained for further requests.
/// </summary>
/// <typeparam name="T"></typeparam>
public class LazyLoad<T> : IFuture<T> {
	private bool _loaded;
	private T _value;
	private readonly Func<T> _loader;

	public LazyLoad(Func<T> valueLoader) {
		_loader = valueLoader;
		_loaded = false;
		_value = default;
	}

	public T Value {
		get {
			if (_loaded)
				return _value;
			_value = _loader();
			_loaded = true;
			return _value;
		}
	}

	public static LazyLoad<T> From(Func<T> valueLoader) {
		return new LazyLoad<T>(valueLoader);
	}

	public override string ToString() {
		return _loaded ? Convert.ToString(_value) : "Future value has not currently been determined";
	}
}
