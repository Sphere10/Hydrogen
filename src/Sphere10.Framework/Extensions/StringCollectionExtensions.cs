//-----------------------------------------------------------------------
// <copyright file="StringCollectionExtensions.cs" company="Sphere 10 Software">
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

using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Sphere10.Framework {
	public static class StringCollectionExtensions {

		public static string ToParagraphCase(this IEnumerable<string> array, bool appendBreaks = false) {
			var paragraph = new ParagraphBuilder();
			array.WithDescriptions().ForEach(s => {
			    paragraph.AppendSentence(s.Item); 
                if (!s.Description.HasFlag(EnumeratedItemDescription.Last))
                    if (appendBreaks) paragraph.AppendParagraphBreak();
			});
			return paragraph.ToString();
		}

		public static IEnumerable<string> TrimMany(this IEnumerable<string> strings, params char[] trimChars) {
			Debug.Assert(strings != null);
			return strings.Select(str => str.Trim(trimChars));
		}

		public static IEnumerable<string> TrimWhitespaceMany(this IEnumerable<string> strings) {
			Debug.Assert(strings != null);
			return strings.TrimMany('\t', ' ', '\r', '\n');
		}




	}
}
