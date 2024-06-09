// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;


namespace Hydrogen;

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
