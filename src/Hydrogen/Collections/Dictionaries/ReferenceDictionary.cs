//-----------------------------------------------------------------------
// <copyright file="ReferenceDictionary.cs" company="Sphere 10 Software">
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
using System.Linq;

namespace Hydrogen {

	public class ReferenceDictionary<TKey, TVal> : Dictionary<Reference<TKey>, TVal> where TKey : class {
	    private readonly Func<TVal, TKey> _keyExtractor;

        public ReferenceDictionary(Func<TVal, TKey> keyExtractor) : this(keyExtractor, Enumerable.Empty<TVal>()) {
        }

	    public ReferenceDictionary(Func<TVal, TKey> keyExtractor, IEnumerable<TVal> items) {
	        _keyExtractor = keyExtractor;
            AddMany(items);
	    }


	    public void AddMany(IEnumerable<TVal> items) {
	        foreach(var item in items)
                Add(Reference.For(_keyExtractor(item)), item);
	    }	
	}

    public class ReferenceDictionary<T> : ReferenceDictionary<T, T> where T : class {
        public ReferenceDictionary() : this(Enumerable.Empty<T>()) {
        }

        public ReferenceDictionary(IEnumerable<T> items) : base((x) => x, items) {
        }
    }
}
