// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Hydrogen;

public static class StringCollectionExtensions {

	public static string ToParagraphCase(this IEnumerable<string> array, bool appendBreaks = false) {
		var paragraph = new ParagraphBuilder();
		array.WithDescriptions().ForEach(s => {
			paragraph.AppendSentence(s.Item);
			if (!s.Description.HasFlag(EnumeratedItemDescription.Last))
				if (appendBreaks)
					paragraph.AppendParagraphBreak();
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
