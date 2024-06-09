// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Hydrogen;

[Serializable, StructLayout(LayoutKind.Sequential)]
public struct SerializableKeyValuePair<TKey, TValue> {
	public SerializableKeyValuePair(KeyValuePair<TKey, TValue> keyValuePair)
		: this(keyValuePair.Key, keyValuePair.Value) {
	}

	public SerializableKeyValuePair(TKey key, TValue value) {
		Key = key;
		Value = value;
	}

	/// <summary>
	/// Gets the Value in the Key/Value Pair
	/// </summary>
	public TValue Value { get; set; }

	/// <summary>
	/// Gets the Key in the Key/Value pair
	/// </summary>
	public TKey Key { get; set; }

	public override string ToString() {
		var builder1 = new StringBuilder();
		builder1.Append('[');
		if (Key != null) {
			builder1.Append(Key);
		}
		builder1.Append(", ");
		if (Value != null) {
			builder1.Append(Value);
		}
		builder1.Append(']');
		return builder1.ToString();
	}
}
