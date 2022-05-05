//-----------------------------------------------------------------------
// <copyright file="SerializableDictionarySurrogate2.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Reflection;

namespace Hydrogen {

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

		[XmlElement]
		public SerializableKeyValuePair<TKey, TValue>[] Items;


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

}
