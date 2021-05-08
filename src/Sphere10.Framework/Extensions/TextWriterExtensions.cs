//-----------------------------------------------------------------------
// <copyright file="TreeExtensions.cs" company="Sphere 10 Software">
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
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	public static class TextWriterExtensions {

		public static void PrintTree<T>(this TextWriter textWriter, T rootNode, Func<T, string> nodeLabel, Func<T, IEnumerable<T>> childrenOf) {
			var firstStack = new List<T> { rootNode };
			var childListStack = new List<List<T>> { firstStack };
			while (childListStack.Count > 0) {
				var childStack = childListStack[^1];

				if (childStack.Count != 0) {
					rootNode = childStack[0];
					childStack.RemoveAt(0);

					var indent = "";
					for (var i = 0; i < childListStack.Count - 1; i++) {
						indent += (childListStack[i].Count > 0) ? "|  " : "   ";
					}

					textWriter.WriteLine(indent + "+- " + nodeLabel(rootNode));
					var children = childrenOf(rootNode);
					if (children.Any()) {
						childListStack.Add(new List<T>(children));
					}
				} else {
					childListStack.RemoveAt(childListStack.Count - 1);
				}
			}
		}
	}

}
