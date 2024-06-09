// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Reflection;

namespace Hydrogen;

[Obfuscation(Exclude = true)]
[XmlRoot("DicionarySurrogate")]
public class SerializableDictionarySurrogate2<TKey, TValue> {

	public SerializableDictionarySurrogate2() {
		Items = new SerializableKeyValuePair<TKey, TValue>[0];
	}

	public SerializableDictionarySurrogate2(IDictionary<TKey, TValue> parent)
		: this() {
		FromDictionary(parent);
	}

	[XmlElement] public SerializableKeyValuePair<TKey, TValue>[] Items;


	public void FromDictionary(IDictionary<TKey, TValue> parent) {
		Items = parent != null ? parent.Select(kv => new SerializableKeyValuePair<TKey, TValue>(kv)).ToArray() : new SerializableKeyValuePair<TKey, TValue>[0];
	}

	public Dictionary<TKey, TValue> ToDictionary() {
		return ToDictionary(new Dictionary<TKey, TValue>()) as Dictionary<TKey, TValue>;
	}

	public IDictionary<TKey, TValue> ToDictionary(IDictionary<TKey, TValue> dictionaryToPopulate) {
		if (Items == null)
			return dictionaryToPopulate;

		foreach (var skv in Items) {
			dictionaryToPopulate.Add(new KeyValuePair<TKey, TValue>(skv.Key, skv.Value));
		}
		return dictionaryToPopulate;
	}
}
