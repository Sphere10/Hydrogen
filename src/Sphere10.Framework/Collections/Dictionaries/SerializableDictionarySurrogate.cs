//-----------------------------------------------------------------------
// <copyright file="SerializableDictionarySurrogate.cs" company="Sphere 10 Software">
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
using System.Xml.Serialization;
using System.Diagnostics;
using System.Reflection;

namespace Sphere10.Framework {

	[Obfuscation(Exclude = true)]
    [XmlRoot("DicionarySurrogate")]
    public class SerializableDictionarySurrogate<TKey, TValue> {
		public SerializableDictionarySurrogate() {
            Keys = new List<TKey>();
            Values = new List<TValue>();
        }

        public SerializableDictionarySurrogate(IDictionary<TKey, TValue> parent)
            : this() {
            FromDictionary(parent);
        }

        [XmlElement("Keys")]
        public List<TKey> Keys {
			get;
			set;
		}

		[XmlElement("Values")]
        public List<TValue> Values {
			get;
			set;
		}

		public void FromDictionary(IDictionary<TKey, TValue> parent) {
            Keys.Clear();
            Values.Clear();
            foreach (var key in parent.Keys) {
                Keys.Add(key);
                Values.Add(parent[key]);
            }
        }

        public IDictionary<TKey, TValue> ToDictionary() {
            return ToDictionary(new Dictionary<TKey, TValue>());
        }

        public IDictionary<TKey, TValue> ToDictionary(IDictionary<TKey, TValue> dictionaryToPopulate) {
            Debug.Assert(Keys.Count == Values.Count);
            dictionaryToPopulate.Clear();
            for (var i = 0; i < Keys.Count; i++) {
                dictionaryToPopulate.Add(
                    Keys[i],
                    Values[i]
                );
            }
            return dictionaryToPopulate;
        }
    }

}
