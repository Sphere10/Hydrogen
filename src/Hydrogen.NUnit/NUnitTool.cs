// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Hydrogen;
using NUnit.Framework.Legacy;

namespace Tools;

public static class NUnit {

	public static bool IsGitHubAction => Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";

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
			ClassicAssert.IsEmpty(collection, message);
		else
			ClassicAssert.IsEmpty(collection);
	}

	public static void IsNotEmpty<T>(IEnumerable<T> collection, string message = null) {
		if (!string.IsNullOrWhiteSpace(message))
			ClassicAssert.IsNotEmpty(collection, message);
		else
			ClassicAssert.IsNotEmpty(collection);
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
