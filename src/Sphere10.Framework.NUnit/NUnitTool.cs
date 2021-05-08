//-----------------------------------------------------------------------
// <copyright file="Tools.NUnit.cs" company="Sphere 10 Software">
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
using System.Text;
using NUnit.Framework;
using Sphere10.Framework;

namespace Tools {
	public static class NUnit {

		public static string Convert2DArrayToString<T>(string header, IEnumerable<IEnumerable<T>> arr2D) {
            var textBuilder = new StringBuilder();
            textBuilder.AppendLine("{0}:", header);
            foreach (var row in arr2D) {
                textBuilder.AppendLine("\t{0}", row.ToDelimittedString(",\t"));
            }
            return textBuilder.ToString();
        }

        public static void IsEmpty<T>(IEnumerable<T> collection, string message = null) {
            if (!string.IsNullOrWhiteSpace(message))
                Assert.IsEmpty(collection, message);
            else
                Assert.IsEmpty(collection);
        }

        public static void IsNotEmpty<T>(IEnumerable<T> collection, string message = null) {
            if (!string.IsNullOrWhiteSpace(message))
                Assert.IsNotEmpty(collection, message);
            else
                Assert.IsNotEmpty(collection);
        }


        public static void Print<T>(IEnumerable<T> items) {
            foreach (var x in items.WithDescriptions()) {
                if (!x.Description.HasFlag(EnumeratedItemDescription.First))
                    Console.Write(", ");
                Console.Write(x.Item);
                if (x.Description.HasFlag(EnumeratedItemDescription.Last))
                    Console.WriteLine();
            }
        }

    }
}
