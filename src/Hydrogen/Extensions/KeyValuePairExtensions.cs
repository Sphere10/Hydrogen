// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace Hydrogen;

public static class KeyValuePairExtensions {
	public static KeyValuePair<V, U> ToInverse<U, V>(this KeyValuePair<U, V> kvp) => new(kvp.Value, kvp.Key);

	public static KeyValuePair<TProjectedKey, TProjectedValue> AsProjection<TKey, TValue, TProjectedKey, TProjectedValue>(
		this KeyValuePair<TKey, TValue> kvp,
		Func<TKey, TProjectedKey> keyProjection,
		Func<TValue, TProjectedValue> valueProjection) => new(keyProjection(kvp.Key), valueProjection(kvp.Value));
}
