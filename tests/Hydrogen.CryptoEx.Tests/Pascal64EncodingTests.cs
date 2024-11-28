// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using Hydrogen.CryptoEx.PascalCoin;
using Hydrogen.NUnit;
using System.Linq;
using NUnit.Framework.Legacy;

namespace Hydrogen.CryptoEx.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class Pascal64EncodingTests {

	#region Pascal64Char

	[Test]
	public void Pascal64StartChar_Unescaped() {
		foreach (var ch in Pascal64Encoding.CharSetStart.Where(c => !Pascal64Encoding.CharSetEscaped.Contains(c)))
			AssertEx.RegexMatch($"{ch}", Pascal64Encoding.StartCharPattern);
	}

	[Test]
	public void Pascal64StartChar_Escaped() {
		foreach (var ch in Pascal64Encoding.CharSetStart.Where(c => Pascal64Encoding.CharSetEscaped.Contains(c)))
			AssertEx.RegexMatch($@"\{ch}", Pascal64Encoding.StartCharPattern);
	}

	[Test]
	public void Pascal64StartChar_IllegalEscape() {
		foreach (var ch in Pascal64Encoding.CharSetStart.Where(c => !Pascal64Encoding.CharSetEscaped.Contains(c)))
			AssertEx.RegexNotMatch($@"\{ch}", Pascal64Encoding.StartCharPattern);
	}

	[Test]
	public void Pascal64StartChar_MissingEscape() {
		foreach (var ch in Pascal64Encoding.CharSetStart.Where(c => Pascal64Encoding.CharSetEscaped.Contains(c)))
			AssertEx.RegexNotMatch($"{ch}", Pascal64Encoding.StartCharPattern);
	}

	[Test]
	public void Pascal64StartChar_IllegalChar() {
		var illegalCharSet = Enumerable
			.Range(0, 255)
			.Select(x => System.Text.Encoding.ASCII.GetString(new[] { (byte)x })[0])
			.Where(c => !Pascal64Encoding.CharSetStart.Contains(c));

		foreach (var ch in illegalCharSet)
			AssertEx.RegexNotMatch($"{ch}", Pascal64Encoding.StartCharPattern);
	}

	[Test]
	public void Pascal64NextChar_Unescaped() {
		foreach (var ch in Pascal64Encoding.CharSetUnescaped)
			AssertEx.RegexMatch($"{ch}", Pascal64Encoding.NextCharPattern);
	}

	[Test]
	public void Pascal64NextChar_Escaped() {
		foreach (var ch in Pascal64Encoding.CharSetEscaped)
			AssertEx.RegexMatch($@"\{ch}", Pascal64Encoding.NextCharPattern);
	}

	[Test]
	public void Pascal64NextChar_IllegalEscape() {
		foreach (var ch in Pascal64Encoding.CharSetUnescaped)
			AssertEx.RegexNotMatch($@"\{ch}", Pascal64Encoding.NextCharPattern);
	}

	[Test]
	public void Pascal64NextChar_MissingEscape() {
		foreach (var ch in Pascal64Encoding.CharSetEscaped)
			AssertEx.RegexNotMatch($"{ch}", Pascal64Encoding.NextCharPattern);
	}

	[Test]
	public void Pascal64NextChar_IllegalChar() {
		var illegalCharSet = Enumerable
			.Range(0, 255)
			.Select(x => System.Text.Encoding.ASCII.GetString(new[] { (byte)x })[0])
			.Where(c => !Pascal64Encoding.CharSet.Contains(c));

		foreach (var ch in illegalCharSet)
			AssertEx.RegexNotMatch($"{ch}", Pascal64Encoding.NextCharPattern);
	}

	#endregion

	#region Pascal64Strings

	[Test]
	public void Pascal64StringPattern_Unescaped() {
		AssertEx.RegexMatch(Pascal64Encoding.CharSetUnescaped, Pascal64Encoding.StringPattern);
	}

	[Test]
	public void Pascal64StringPattern_Escaped() {
		var badInput = @"correctly\[\]escaped";
		AssertEx.RegexMatch(badInput, Pascal64Encoding.StringPattern);
	}

	[Test]
	public void Pascal64StringPattern_FullCharSet_Escaped() {
		AssertEx.RegexMatch(Pascal64Encoding.Escape(Pascal64Encoding.CharSet), PascalAsciiEncoding.StringPattern);
	}

	[Test]
	public void Pascal64StringPattern_IllegalEscape() {
		var badInput = @"illegal\escape";
		AssertEx.RegexNotMatch(badInput, Pascal64Encoding.StringPattern);
	}

	[Test]
	public void Pascal64StringPattern_MissingEscape() {
		var badInput = @"missing[]escape";
		AssertEx.RegexNotMatch(badInput, Pascal64Encoding.StringPattern);
	}

	[Test]
	public void Pascal64StringPattern_IllegalCharSet() {
		var illegalCharSet =
			Enumerable
				.Range(0, 255)
				.Select(x => System.Text.Encoding.ASCII.GetString(new[] { (byte)x })[0])
				.Where(c => !Pascal64Encoding.CharSet.Contains(c))
				.ToDelimittedString(string.Empty);

		foreach (var subset in illegalCharSet.Partition(63)) {
			var testSet = "b" + new string(subset.ToArray());
			AssertEx.RegexNotMatch(testSet, Pascal64Encoding.StringPattern);
		}
	}

	[Test]
	public void Pascal64StringPattern_Short() {
		var badInput = @"@";
		AssertEx.RegexMatch(badInput, Pascal64Encoding.StringPattern);
	}

	[Test]
	public void Pascal64StringPattern_Long() {
		var badInput = Enumerable.Range(0, 256).Select(c => 'b').ToDelimittedString(string.Empty);
		AssertEx.RegexMatch(badInput, Pascal64Encoding.StringPattern);
	}

	#endregion

	#region Misc

	[Test]
	public void Encoding_EscapeString() {
		var escaped = @"\(a\)b\{c\}d\[e\]f\:g\""h\<i\>";
		var unescaped = @"(a)b{c}d[e]f:g""h<i>";
		ClassicAssert.AreEqual(escaped, Pascal64Encoding.Escape(unescaped));
		ClassicAssert.AreEqual(escaped, Pascal64Encoding.Escape(Pascal64Encoding.Escape(unescaped)));
		ClassicAssert.IsFalse(Pascal64Encoding.IsValidEscaped(unescaped));
		ClassicAssert.IsTrue(Pascal64Encoding.IsValidEscaped(escaped));
	}

	[Test]
	public void Encoding_UnescapedString() {
		var escaped = @"\(a\)b\{c\}d\[e\]f\:g\""h\<i\>";
		var unescaped = @"(a)b{c}d[e]f:g""h<i>";
		ClassicAssert.AreEqual(unescaped, Pascal64Encoding.Unescape(escaped));
		ClassicAssert.AreEqual(unescaped, Pascal64Encoding.Unescape(Pascal64Encoding.Unescape(escaped)));
		ClassicAssert.IsFalse(Pascal64Encoding.IsValidUnescaped(escaped));
		ClassicAssert.IsTrue(Pascal64Encoding.IsValidUnescaped(unescaped));
	}

	#endregion

}
