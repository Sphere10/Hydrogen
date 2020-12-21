//-----------------------------------------------------------------------
// <copyright file="CollectionGroup.cs" company="Sphere 10 Software">
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

    public class CollectionGroup<TKey, TElement, TCollection> : IGrouping<TKey, TElement> where TCollection : ICollection<TElement>, new(){
        
        public CollectionGroup(TKey key, IEnumerable<TElement> elements) {
            Key = key;
            Elements = new TCollection();
            elements.ForEach(Elements.Add); 
        }

		public TKey Key { get; }

		public TCollection Elements { get; }

		public IEnumerator<TElement> GetEnumerator() {
            return Elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    public class CollectionGroup<TKey, TElement> : CollectionGroup<TKey, TElement, List<TElement>> {
        public CollectionGroup(TKey key, IEnumerable<TElement> elements)
            : base(key, elements) {
        }
    }
}
