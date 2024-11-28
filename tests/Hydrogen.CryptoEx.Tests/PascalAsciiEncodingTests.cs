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
public class PascalAsciiEncodingTests {

	#region PascalAsciiChar

	[Test]
	public void PascalAsciiCharPattern_Unescaped() {
		foreach (var ch in PascalAsciiEncoding.CharSetUnescaped)
			AssertEx.RegexMatch($"{ch}", PascalAsciiEncoding.CharPattern);
	}

	[Test]
	public void PascalAsciiCharPattern_Escaped() {
		foreach (var ch in PascalAsciiEncoding.CharSetEscaped)
			AssertEx.RegexMatch($@"\{ch}", PascalAsciiEncoding.CharPattern);
	}

	[Test]
	public void PascalAsciiCharPattern_IllegalEscape() {
		foreach (var ch in PascalAsciiEncoding.CharSetUnescaped)
			AssertEx.RegexNotMatch($"{PascalAsciiEncoding.EscapeChar}{ch}", PascalAsciiEncoding.CharPattern);
	}

	[Test]
	public void PascalAsciiCharPattern_MissingEscape() {
		foreach (var ch in PascalAsciiEncoding.CharSetEscaped)
			AssertEx.RegexNotMatch($"{ch}", PascalAsciiEncoding.CharPattern);
	}

	[Test]
	public void PascalAsciiCharPattern_IllegalChar() {
		var illegalCharSet = Enumerable
			.Range(0, 255)
			.Select(x => System.Text.Encoding.ASCII.GetString(new[] { (byte)x })[0])
			.Where(c => !PascalAsciiEncoding.CharSet.Contains(c));

		foreach (var ch in illegalCharSet)
			AssertEx.RegexNotMatch($"{ch}", PascalAsciiEncoding.CharPattern);
	}

	#endregion

	#region PascalAsciiString

	[Test]
	public void PascalAsciiStringPattern_Unescaped() {
		AssertEx.RegexMatch(PascalAsciiEncoding.CharSetUnescaped, PascalAsciiEncoding.StringPattern);
	}

	[Test]
	public void PascalAsciiStringPattern_Escaped() {
		var input = @"proper\\escape";
		AssertEx.RegexMatch(input, PascalAsciiEncoding.StringPattern);
	}

	[Test]
	public void PascalAsciiStringPattern_FullCharSet_Escaped() {
		AssertEx.RegexMatch(PascalAsciiEncoding.Escape(PascalAsciiEncoding.CharSet), PascalAsciiEncoding.StringPattern);
	}

	[Test]
	public void PascalAsciiStringPattern_IllegalEscape() {
		var badInput = @"illegal\escape";
		AssertEx.RegexNotMatch(badInput, PascalAsciiEncoding.StringPattern);
	}

	[Test]
	public void PascalAsciiStringPattern_MissingEscape() {
		var badInput = @"missing[]escape";
		AssertEx.RegexNotMatch(badInput, PascalAsciiEncoding.StringPattern);
	}

	[Test]
	public void PascalAsciiStringPattern_IllegalCharSet() {
		var illegalCharSet =
			Enumerable
				.Range(0, 255)
				.Select(x => System.Text.Encoding.ASCII.GetString(new[] { (byte)x })[0])
				.Where(c => !PascalAsciiEncoding.CharSet.Contains(c))
				.ToDelimittedString(string.Empty);

		AssertEx.RegexNotMatch(illegalCharSet, PascalAsciiEncoding.StringPattern);
		AssertEx.RegexNotMatch(PascalAsciiEncoding.CharSetUnescaped + illegalCharSet, PascalAsciiEncoding.StringPattern);
		AssertEx.RegexNotMatch(illegalCharSet + PascalAsciiEncoding.CharSetUnescaped, PascalAsciiEncoding.StringPattern);
	}

	#endregion

	#region Misc

	[Test]
	public void Encoding_EscapeString() {
		var unescaped = @"""a(b)c:d<e>f[g\h]i{j}";
		var escaped = @"\""a\(b\)c\:d\<e\>f\[g\\h\]i\{j\}";
		ClassicAssert.AreEqual(escaped, PascalAsciiEncoding.Escape(unescaped));
		ClassicAssert.AreEqual(escaped, PascalAsciiEncoding.Escape(PascalAsciiEncoding.Escape(unescaped)));
		ClassicAssert.IsTrue(PascalAsciiEncoding.IsValidEscaped(escaped));
		ClassicAssert.IsTrue(PascalAsciiEncoding.IsValidEscaped(unescaped)); // unescaped is also a valid escaped string (unlike pascal64 encoding)
	}

	[Test]
	public void Encoding_UnescapedString() {
		var unescaped = @"""a(b)c:d<e>f[g\h]i{j}";
		var escaped = @"\""a\(b\)c\:d\<e\>f\[g\\h\]i\{j\}";
		ClassicAssert.AreEqual(unescaped, PascalAsciiEncoding.Unescape(escaped));
		ClassicAssert.AreEqual(unescaped, PascalAsciiEncoding.Unescape(PascalAsciiEncoding.Unescape(escaped)));
		ClassicAssert.IsTrue(PascalAsciiEncoding.IsValidUnescaped(escaped)); // escaped string is also valid as unescaped, since \ is allowed
		ClassicAssert.IsTrue(PascalAsciiEncoding.IsValidUnescaped(unescaped));
	}

	#endregion

}
