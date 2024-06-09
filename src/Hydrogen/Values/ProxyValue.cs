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
/// A future whose value is fetched on every request.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ProxyValue<T> : IFuture<T> {
	private readonly Func<T> _loader;

	public ProxyValue(Func<T> valueLoader) {
		_loader = valueLoader;
	}

	public T Value => _loader();

	public static ProxyValue<T> From(Func<T> valueLoader) {
		return new ProxyValue<T>(valueLoader);
	}

	public override string ToString() => Value.ToString();

}
