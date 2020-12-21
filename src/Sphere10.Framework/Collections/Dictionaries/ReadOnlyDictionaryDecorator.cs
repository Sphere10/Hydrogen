//-----------------------------------------------------------------------
// <copyright file="ReadOnlyDictionaryDecorator.cs" company="Sphere 10 Software">
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

using System;
using System.Collections.Generic;

namespace Sphere10.Framework {

    public class ReadOnlyDictionaryDecorator<K,V> : DictionaryDecorator<K,V> {

        public ReadOnlyDictionaryDecorator(IDictionary<K,V> dictionary ) : base(dictionary) {
                
        }
        public override void Add(K key, V value) {
            ThrowError();
            
        }

        public override void Add(KeyValuePair<K, V> item) {
            ThrowError();
        }

        public override void Clear() {
            ThrowError();
        }

        public override bool Remove(K item) {
            ThrowError();
            return false;
        }

        public override bool Remove(KeyValuePair<K, V> item) {
            ThrowError();
            return false;
        }

        public override V this[K key] {
            get => base[key];
			set => ThrowError();
		}

        public override bool IsReadOnly => true;

		private static void ThrowError() {
            throw new Exception("Dictionary is readonly");
        }
    }
}
