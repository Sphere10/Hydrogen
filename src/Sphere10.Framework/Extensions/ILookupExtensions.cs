//-----------------------------------------------------------------------
// <copyright file="ILookupExtensions.cs" company="Sphere 10 Software">
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
using System.Text;



namespace Sphere10.Framework {

	public static class ILookupExtensions {
		public static ILookup<TKey, TValueOut> Transform<TKey, TValue, TValueOut>(
			   this ILookup<TKey, TValue> lookup,
			   Func<TValue, TValueOut> selector) {
            // NOTE: does not call method below since it is not tested and DataSync uses this!
			return lookup.
				   SelectMany(g => g,
							  (g, v) => new KeyValuePair<TKey, TValueOut>(g.Key, selector(v))).
				   ToLookup(kvp => kvp.Key, kvp => kvp.Value);
		}


        public static ILookup<TKeyOut, TValueOut> Transform<TKey, TValue, TKeyOut, TValueOut>(
            this ILookup<TKey, TValue> lookup, 
            Func<TKey, TKeyOut> keyTransformer,
            Func<TValue, TValueOut> valueTransformer) {

            var lookupEx = new LookupEx<TKeyOut, TValueOut>();
            foreach (var grouping in lookup)
                lookupEx[keyTransformer(grouping.Key)] = grouping.Select(valueTransformer);

            return lookupEx;
        }
	}
}
