// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StringFormatterTests {

	[Test]
	public void SimpleTest_1() {
		Assert.AreEqual(string.Format("{0}", 1), Tools.Text.FormatEx("{0}", 1));
	}

	[Test]
	public void SimpleTest_2() {
		Assert.AreEqual(string.Format(" {0}", 23), Tools.Text.FormatEx(" {0}", 23));
	}

	public void SimpleTest_3() {
		Assert.AreEqual(string.Format(" {0}", 23), Tools.Text.FormatEx(" {0}", 23));
	}

	[Test]
	public void SimpleTest_4() {
		Assert.AreEqual(string.Format(" !{0}! ", 99), Tools.Text.FormatEx(" !{0}! ", 99));
	}

	[Test]
	public void SimpleTest_5() {
		var now = DateTime.Now;
		Assert.AreEqual(string.Format(" x{0:yyyy-MM-dd}x ", now), Tools.Text.FormatEx(" x{0:yyyy-MM-dd}x ", now));
	}

	[Test]
	public void SimpleTest_6() {
		var now = DateTime.Now;
		Assert.AreEqual(string.Format(" ${0:yyyy-MM-dd}^^^^^{1}{2:HH:mm:ss tt zz}! ", now, "ALPHA", now), Tools.Text.FormatEx(" ${0:yyyy-MM-dd}^^^^^{1}{2:HH:mm:ss tt zz}! ", now, "ALPHA", now));
	}

	[Test]
	public void LookupTest_1() {
		Func<string, string> resolver = (token) => {
			switch (token) {
				case "token":
					return "1";
				default:
					return null;
			}
		};
		Assert.AreEqual("1", Tools.Text.FormatEx("{token}", resolver));
	}

	[Test]
	public void LookupTest_2() {
		Func<string, string> resolver = (token) => {
			switch (token) {
				case "token1":
					return "1";
				case "token2":
					return "2";
				default:
					return null;
			}
		};
		Assert.AreEqual("   X1lhjk34k2342kj4h2!", Tools.Text.FormatEx("   X{token1}lhjk34k2342kj4h{token2}!", resolver));
	}

	[Test]
	public void MixedTest_1() {
		Func<string, string> resolver = (token) => {
			switch (token) {
				case "token1":
					return "B";
				case "token2":
					return "D";
				default:
					return null;
			}
		};
		Assert.AreEqual(" !ABCDEEDCBA! ", Tools.Text.FormatEx(" !{0}{token1}{1}{token2}{2}{2}{token2}{1}{token1}{0}! ", resolver, "A", "C", "E"));
	}

	[Test]
	public void EscapedTest_1() {
		Assert.AreEqual(string.Format("{{"), Tools.Text.FormatEx("{{"));
	}

	[Test]
	public void EscapedTest_2() {
		Assert.AreEqual(string.Format("{{"), Tools.Text.FormatEx("{{"));
	}

	[Test]
	public void EscapedTest_3() {
		Assert.AreEqual(string.Format("{{0}}"), Tools.Text.FormatEx("{{0}}"));
	}

	[Test]
	public void EscapedTest_4() {
		Assert.AreEqual(string.Format("{{{0}", 1), Tools.Text.FormatEx("{{{0}", 1));
	}

	[Test]
	public void EscapedTest_5() {
		Assert.AreEqual(string.Format("{0}}}", 1), Tools.Text.FormatEx("{0}}}", 1));
	}

	[Test]
	public void EscapedTest_6() {
		Assert.AreEqual(string.Format("{{{0}}}", 1), Tools.Text.FormatEx("{{{0}}}", 1));
	}

	[Test]
	public void EscapedTest_7() {
		Assert.AreEqual(string.Format("{{}}", 1), Tools.Text.FormatEx("{{}}", 1));
	}

	[Test]
	public void EscapedTest_8() {
		Func<string, string> resolver = (token) => {
			switch (token) {
				case "token1":
					return "B";
				case "token2":
					return "D";
				default:
					return null;
			}
		};
		Assert.AreEqual(" {!A{BCD}EEDCBA!} ", Tools.Text.FormatEx(" {{!{0}{{{token1}{1}{token2}}}{2}{2}{token2}{1}{token1}{0}!}} ", resolver, "A", "C", "E"));
	}

	[Test]
	public void DanglingBraces_1() {
		Assert.Throws<FormatException>(() => string.Format("{"));
		Assert.DoesNotThrow(() => Tools.Text.FormatEx("{"));
	}

	[Test]
	public void DanglingBraces_2() {
		Assert.Throws<FormatException>(() => string.Format("}"));
		Assert.DoesNotThrow(() => Tools.Text.FormatEx("}"));
	}

	[Test]
	public void DanglingBraces_3() {
		Assert.Throws<FormatException>(() => string.Format("{{}}}"));
		Assert.DoesNotThrow(() => Tools.Text.FormatEx("{{}}}"));
	}

	[Test]
	public void DanglingBraces_4() {
		Assert.Throws<FormatException>(() => string.Format("{{}}{"));
		Assert.DoesNotThrow(() => Tools.Text.FormatEx("{{}}{"));
	}

	[Test]
	public void DanglingBraces_5() {
		Assert.Throws<FormatException>(() => string.Format("{{}}}"));
		Assert.DoesNotThrow(() => Tools.Text.FormatEx("{{{}}"));
	}

	[Test]
	public void FormatWithDictionary_NoRecursionDoesntRecurse() {
		var tokens = new Dictionary<string, object> {
			["A"] = "{B}{A}",
			["B"] = "{A}{B}",
		};
		Assert.That(StringFormatter.FormatWithDictionary("{A}", tokens, false), Is.EqualTo("{B}{A}"));
		Assert.That(StringFormatter.FormatWithDictionary("{B}", tokens, false), Is.EqualTo("{A}{B}"));
	}

	[Test]
	public void FormatWithDictionary_HandlesRecursiveLoop() {
		var tokens = new Dictionary<string, object> {
			["A"] = "{B}{A}",
			["B"] = "{A}{B}",
		};
		Assert.That(StringFormatter.FormatWithDictionary("{A}", tokens, true), Is.EqualTo("{A}{B}{A}"));
		Assert.That(StringFormatter.FormatWithDictionary("{B}", tokens, true), Is.EqualTo("{B}{A}{B}"));
	}

	[Test]
	public void FormatWithDictionary_HandlesRecursiveTokenName() {
		var tokens = new Dictionary<string, object> {
			["Alpha_Beta"] = "Found it!",
			["B"] = "Beta",
		};
		Assert.That(StringFormatter.FormatWithDictionary("{Alpha_{B}" + "}", tokens, true), Is.EqualTo("Found it!"));
	}

	[Test]
	public void FormatWithDictionary_NoRecursionOnTokenName() {
		var tokens = new Dictionary<string, object> {
			["Alpha_Beta"] = "Found it!",
			["B"] = "Beta",
		};
		Assert.That(StringFormatter.FormatWithDictionary("{Alpha_{B}" + "}", tokens, false), Is.EqualTo("{Alpha_{B}}"));
	}

	[Test]
	public void DoesNotChangeNestedTokenWhenNotRecursive() {
		var tokens = new Dictionary<string, object> {
			["A"] = "B",
			["B"] = "Success",
		};
		Assert.That(StringFormatter.FormatWithDictionary("{ {A} }", tokens, false), Is.EqualTo("{ {A} }"));
	}

	[Test]
	public void HandlesNestedTokensWhenRecursive_1() {
		var tokens = new Dictionary<string, object> {
			["A"] = "B",
		};
		Assert.That(StringFormatter.FormatWithDictionary("{ {A} }", tokens, true), Is.EqualTo("{ B }"));

	}


	[Test]
	public void HandlesNestedTokensWhenRecursive_2() {
		var tokens = new Dictionary<string, object> {
			["A"] = "B",
			["B"] = "Success",
		};
		Assert.That(StringFormatter.FormatWithDictionary("{ {A} }{ {A} }", tokens, true), Is.EqualTo("SuccessSuccess"));

	}


	[Test]
	public void HandlesNestedTokensWhenRecursive_3() {
		var tokens = new Dictionary<string, object> {
			["A"] = "B",
		};
		Assert.That(StringFormatter.FormatWithDictionary("{{{A}}}", tokens, true), Is.EqualTo("{B}"));

	}


	[Test]
	public void HandlesNestedTokensWhenRecursive_4() {
		var tokens = new Dictionary<string, object> {
			["A"] = "B",
			["B"] = "Success",
		};
		Assert.That(StringFormatter.FormatWithDictionary("{ {A} }", tokens, true), Is.EqualTo("Success"));

	}


	/*
	 {"stringsElement": "#typed-strings-{object_id}", "startDelay": 500, "backSpeed": 15, "backDelay":5000}
	
	{"stringsElement": "#typed-strings-{object_id}", "startDelay": 500, "backSpeed": 15, "backDelay":5000, "stringsElement2": "#typed-strings2-{object_id}"}

	 */

	[Test]
	public void LocalNotionBugCase_1() {
		var tokens = new Dictionary<string, object> {
			["object_id"] = "309c96b2-5ac0-48d0-b75e-502ce962baf2",
		};
		var problematicString = "{\"stringsElement\": \"#typed-strings-{object_id}\", \"startDelay\": 500, \"backSpeed\": 15, \"backDelay\":5000}";
		var expectedString = "{\"stringsElement\": \"#typed-strings-309c96b2-5ac0-48d0-b75e-502ce962baf2\", \"startDelay\": 500, \"backSpeed\": 15, \"backDelay\":5000}";
		Assert.That(StringFormatter.FormatWithDictionary(problematicString, tokens, true), Is.EqualTo(expectedString));

	}

	[Test]
	public void LocalNotionBugCase_1_Variant() {
		var tokens = new Dictionary<string, object> {
			["object_id"] = "309c96b2-5ac0-48d0-b75e-502ce962baf2",
		};
		var problematicString = "{\"stringsElement\": \"#typed-strings-{object_id}\", \"startDelay\": 500, \"backSpeed\": 15, \"backDelay\":5000, \"stringsElement2\": \"#typed-strings2-{object_id}\"}";
		var expectedString = "{\"stringsElement\": \"#typed-strings-309c96b2-5ac0-48d0-b75e-502ce962baf2\", \"startDelay\": 500, \"backSpeed\": 15, \"backDelay\":5000, \"stringsElement2\": \"#typed-strings2-309c96b2-5ac0-48d0-b75e-502ce962baf2\"}";
		Assert.That(StringFormatter.FormatWithDictionary(problematicString, tokens, true), Is.EqualTo(expectedString));
	}


	[Test]
	public void LocalNotionBugCase_1_Variant_2() {
		var tokens = new Dictionary<string, object> {
			["A"] = Tools.Values.Future.AlwaysLoad(() => "{\"stringsElement\": \"#typed-strings-{object_id}\", \"startDelay\": 500, \"backSpeed\": 15, \"backDelay\":5000, \"stringsElement2\": \"#typed-strings2-{object_id}\"}"),
			["object_id"] = "309c96b2-5ac0-48d0-b75e-502ce962baf2",
		};
		var problematicString = "{A}";
		var expectedString = "{\"stringsElement\": \"#typed-strings-309c96b2-5ac0-48d0-b75e-502ce962baf2\", \"startDelay\": 500, \"backSpeed\": 15, \"backDelay\":5000, \"stringsElement2\": \"#typed-strings2-309c96b2-5ac0-48d0-b75e-502ce962baf2\"}";
		Assert.That(StringFormatter.FormatWithDictionary(problematicString, tokens, true), Is.EqualTo(expectedString));
	}


	[Test]
	public void LocalNotionBugCase_1_Variant_3() {
		var tokens = new Dictionary<string, object> {
			["A"] = Tools.Values.Future.AlwaysLoad(() => "{\"stringsElement\": \"#typed-strings-{object_id}\", \"startDelay\": 500, \"backSpeed\": 15, \"backDelay\":5000, \"stringsElement2\": \"#typed-strings2-{object_id}\"}"),
			["object_id"] = Tools.Values.Future.AlwaysLoad(() => "309c96b2-5ac0-48d0-b75e-502ce962baf2"),
		};
		var problematicString = "Some text with {A}";
		var expectedString =
			"Some text with {\"stringsElement\": \"#typed-strings-309c96b2-5ac0-48d0-b75e-502ce962baf2\", \"startDelay\": 500, \"backSpeed\": 15, \"backDelay\":5000, \"stringsElement2\": \"#typed-strings2-309c96b2-5ac0-48d0-b75e-502ce962baf2\"}";
		Assert.That(StringFormatter.FormatWithDictionary(problematicString, tokens, true), Is.EqualTo(expectedString));
	}

	[Test]
	public void LocalNotionBugCase_2() {
		var input =
			"""
				function MakeDownloadLink(target) {
					var text = $(target).html().toUpperCase();
					var isWindows = text.startsWith("WIN");
					var isMacOs = text.startsWith("MAC");
					var isLinux = text.startsWith("LIN");
					var isDownload = isWindows || isMacOS || isLinux;
					if (isDownload) {
						$(target).attr('class', 'btn btn-primary me-1 lift');
					}
				}

				$(document).ready(function() {
					$("a").each( function() {
						MakeDownloadLink(this);
					});
				});
				""";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_1() {
		var input = "{ { } }";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}


	[Test]
	public void LocalNotionBugCase_2_Variant_2() {
		var input = "{ }";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_3() {
		var input = " { } ";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_4() {
		var input = " {} ";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_5() {
		var input = "{";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_6() {
		var input = "{ {";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_7() {
		var input = "{ {}";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_8() {
		var input = "{ { } ";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_9() {
		var input = "}";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_10() {
		var input = "} }";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_11() {
		var input = "{} }";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_12() {
		var input = "}{";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_13() {
		var input = "}{ { ";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}


	[Test]
	public void LocalNotionBugCase_2_Variant_14() {
		var input = "}{ {  {   }";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}


	[Test]
	public void LocalNotionBugCase_2_Variant_15() {
		var input = "}{ {  {   } } { { }{}{  { } }";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_16() {
		var input = "}{ {  {   } } { { }{}{  { } }{ {} } } { }{} }{}{} }{} }{    } {";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_17() {
		var input = "}{ {  {   } } { { }{}{  { } }{ {} } } { }{} }";
		//            1 2  3   2 1 2 3 2323  4 3 23 43 2 1 2 121 0 	
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_18() {
		var input = "  {   } { {}}";
		//            1 2  3   2 1 2 3 2323  4 3 23 43 2 1 2 121 0 	
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}


	[Test]
	public void LocalNotionBugCase_2_Variant_19() {
		var input = "{}{ {} }";
		Assert.That(StringFormatter.FormatWithDictionary(input, new Dictionary<string, object> { ["foo"] = "bar" }, true), Is.EqualTo(input));
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_Stress([Values(0, 1, 11, 17, 97)] int size, [Values(0, 1, 11, 17, 97)] int rounds) {
		var inputChars = new char[size];
		var rng = new Random(31337);
		for (var i = 0; i < rounds; i++) {
			for (var j = 0; j < size; j++)
				inputChars[j] = ' ';
			var numOpenBrance = rng.Next(0, size);
			var numCloseBrance = rng.Next(0, size);
			for (var j = 0; j < numOpenBrance; j++)
				inputChars[rng.Next(0, size)] = '{';
			for (var j = 0; j < numCloseBrance; j++)
				inputChars[rng.Next(0, size)] = '}';

			var input = new string(inputChars);

			// Keep reducing string from escape { and }
			while (input.Length != (input = input.Replace("{{", "{").Replace("}}", "}")).Length) ;
			var expected = input;
			var actual = StringFormatter.FormatWithDictionary(expected, new Dictionary<string, object> { ["foo"] = "bar" }, true);
			Assert.That(actual, Is.EqualTo(expected));
		}
	}


	[Test]
	public void LocalNotionBugCase_2_Variant_Finder_3Char() {
		var inputChars = new char[3];
		var charset = new[] { ' ', '{', '}' };

		foreach (var char1 in charset)
		foreach (var char2 in charset)
		foreach (var char3 in charset) {
			var input = new string(new[] { char1, char2, char3 });
			while (input.Length != (input = input.Replace("{{", "{").Replace("}}", "}")).Length) ;
			var expected = input;
			var actual = StringFormatter.FormatWithDictionary(expected, new Dictionary<string, object> { ["foo"] = "bar" }, true);
			Assert.That(actual, Is.EqualTo(expected));

		}

	}

	[Test]
	public void LocalNotionBugCase_2_Variant_Finder_5Char() {
		var inputChars = new char[5];
		var charset = new[] { ' ', '{', '}' };

		foreach (var char1 in charset)
		foreach (var char2 in charset)
		foreach (var char3 in charset)
		foreach (var char4 in charset)
		foreach (var char5 in charset) {
			var input = new string(new[] { char1, char2, char3, char4, char5 });
			while (input.Length != (input = input.Replace("{{", "{").Replace("}}", "}")).Length) ;
			var expected = input;
			var actual = StringFormatter.FormatWithDictionary(expected, new Dictionary<string, object> { ["foo"] = "bar" }, true);
			Assert.That(actual, Is.EqualTo(expected));

		}

	}

	[Test]
	public void LocalNotionBugCase_2_Variant_Finder_6Char() {
		var inputChars = new char[5];
		var charset = new[] { ' ', '{', '}' };

		foreach (var char1 in charset)
		foreach (var char2 in charset)
		foreach (var char3 in charset)
		foreach (var char4 in charset)
		foreach (var char5 in charset)
		foreach (var char6 in charset) {
			var input = new string(new[] { char1, char2, char3, char4, char5, char6 });
			while (input.Length != (input = input.Replace("{{", "{").Replace("}}", "}")).Length) ;
			var expected = input;
			var actual = StringFormatter.FormatWithDictionary(expected, new Dictionary<string, object> { ["foo"] = "bar" }, true);
			Assert.That(actual, Is.EqualTo(expected));

		}
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_Finder_7Char() {
		var inputChars = new char[5];
		var charset = new[] { ' ', '{', '}' };

		foreach (var char1 in charset)
		foreach (var char2 in charset)
		foreach (var char3 in charset)
		foreach (var char4 in charset)
		foreach (var char5 in charset)
		foreach (var char6 in charset)
		foreach (var char7 in charset) {
			var input = new string(new[] { char1, char2, char3, char4, char5, char6, char7 });
			while (input.Length != (input = input.Replace("{{", "{").Replace("}}", "}")).Length) ;
			var expected = input;
			var actual = StringFormatter.FormatWithDictionary(expected, new Dictionary<string, object> { ["foo"] = "bar" }, true);
			Assert.That(actual, Is.EqualTo(expected));

		}
	}

	[Test]
	public void LocalNotionBugCase_2_Variant_Finder_8Char() {
		var inputChars = new char[5];
		var charset = new[] { ' ', '{', '}' };

		foreach (var char1 in charset)
		foreach (var char2 in charset)
		foreach (var char3 in charset)
		foreach (var char4 in charset)
		foreach (var char5 in charset)
		foreach (var char6 in charset)
		foreach (var char7 in charset)
		foreach (var char8 in charset) {
			var input = new string(new[] { char1, char2, char3, char4, char5, char6, char7, char8 });
			while (input.Length != (input = input.Replace("{{", "{").Replace("}}", "}")).Length) ;
			var expected = input;
			var actual = StringFormatter.FormatWithDictionary(expected, new Dictionary<string, object> { ["foo"] = "bar" }, true);
			Assert.That(actual, Is.EqualTo(expected));

		}
	}
}
