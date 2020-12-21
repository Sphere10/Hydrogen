//-----------------------------------------------------------------------
// <copyright file="LinkedListExtensions.cs" company="Sphere 10 Software">
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

    public static class LinkedListExtensions {

        public static LinkedListNode<T> Find<T>(this LinkedList<T> linkedList, Predicate<T> predicate) {
            if (linkedList.Count > 0) {
                var currNode = linkedList.First;
                while (currNode != null) {
                    if (predicate(currNode.Value))
                        return currNode;
                    currNode = currNode.Next;
                }                
            }
            return null;
        }  

    }
}
