//-----------------------------------------------------------------------
// <copyright file="ICollectionExtensions.cs" company="Sphere 10 Software">
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



namespace Sphere10.Framework {

	public static class ICollectionExtensions {


        public static void AddRangeSequentially<T>(this ICollection<T> collection, IEnumerable<T> items) {
            items.Update(collection.Add);
        }

        /// <summary>
        /// Removes all the items from the collection matching the given predicate. 
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="collection">The collection</param>
        /// <param name="predicate">The predicate</param>
        /// <returns>The removed items</returns>
        public static IEnumerable<T> RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> predicate) {
            var items = collection.Where(predicate).Select(x => x).ToArray();
            foreach (var item in items)
                collection.Remove(item);
            return items;
        }

    }
}
