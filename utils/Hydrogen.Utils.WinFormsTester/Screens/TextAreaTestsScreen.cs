// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Hydrogen.Windows.Forms;
using Tuple = System.Tuple;

namespace Hydrogen.Utils.WinFormsTester;

public partial class TextAreaTestsScreen : ApplicationScreen {
	public TextAreaTestsScreen() {
		InitializeComponent();
	}
	private void _fillStandardButton_Click(object sender, EventArgs e) {
		try {
			_standardTextBox.Clear();
			var start = DateTime.Now;
			long genSize = 0;
			while (genSize < 1000000) {
				var randomString = Tools.Text.GenerateRandomString(50) + Environment.NewLine;
				genSize += randomString.Length * sizeof(char);
				_standardTextBox.AppendText(randomString);
			}
			DialogEx.Show(this, SystemIconType.Information, "Results", "{0} bytes took {1} seconds".FormatWith(genSize, DateTime.Now.Subtract(start).TotalSeconds));
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}

	private void _fillLockedButton_Click(object sender, EventArgs e) {
		//try {
		//    _standardTextBox.Clear();
		//    var start = DateTime.Now;
		//    long genSize = 0;
		//    if (!WinAPI.USER32.LockWindowUpdate(_lockTextBox.Handle)) {
		//        throw new Exception("Unable to lock");
		//    }
		//    while (genSize < 1000000) {
		//        var randomString = Tools.Text.GenerateRandomString(50) + Environment.NewLine;
		//        genSize += randomString.Length * sizeof(char);
		//        _lockTextBox.AppendText(randomString);
		//    }
		//    if (!WinAPI.USER32.LockWindowUpdate(IntPtr.Zero)) {
		//        throw new Exception("Unable to unlock");
		//    }

		//    DialogEx.Show(this, SystemIconType.Information, "Results", "{0} bytes took {1} seconds".FormatWith(genSize, DateTime.Now.Subtract(start).TotalSeconds));
		//} catch (Exception error) {
		//    ExceptionDialog.Show(error);
		//}
	}

	private void _epasaPwdCharsButton_Click(object sender, EventArgs e) {
		const string regexEscapes = @"[\^$.|?*+(){}";
		const string epasaEscapes = @":\""[]()<>{}";
		const string csharpEscapes = @"""";
		const string pascal64charset = @"abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-+{}[]_:`|<>,.?/~";


		var sb = new StringBuilder();
		for (var i = 32; i <= 126; i++)
			sb.Append($"\"{Encoding.ASCII.GetString(new byte[] { (byte)i })}\" | ");

		sb.AppendLine();
		sb.Append("ANSICHAR = (");
		for (var i = 32; i <= 126; i++) {
			var charStr = Encoding.ASCII.GetString(new byte[] { (byte)i });
			var regexEscaped = regexEscapes.Contains(charStr);
			var epasaEscaped = epasaEscapes.Contains(charStr);
			var csharpEscaped = csharpEscapes.Contains(charStr);
			var prefix = string.Empty;
			if (regexEscaped)
				prefix = @"\";
			if (epasaEscaped)
				prefix = @"\\" + prefix;
			if (csharpEscaped)
				charStr = @"""""";

			charStr = prefix + charStr;
			sb.Append($"{charStr}|");
		}

		sb.Append(")");

		sb.AppendLine();
		sb.Append("PASCAL64 = (");
		foreach (var ch in pascal64charset) {
			var charStr = ch.ToString();
			var regexEscaped = regexEscapes.Contains(charStr);
			var epasaEscaped = epasaEscapes.Contains(charStr);
			var csharpEscaped = csharpEscapes.Contains(charStr);
			var prefix = string.Empty;
			if (regexEscaped)
				prefix = @"\";
			if (epasaEscaped)
				prefix = @"\\" + prefix;
			if (csharpEscaped)
				charStr = @"""""";

			charStr = prefix + charStr;
			sb.Append($"{charStr}|");
		}

		sb.Append(")");


		sb.AppendLine();
		sb.Append(@"EscapedAnsiCharSet = @""");
		for (var i = 32; i <= 126; i++) {
			var ch = Encoding.ASCII.GetString(new byte[] { (byte)i }).ToString();
			if (csharpEscapes.Contains(ch))
				ch = @"""";
			if (epasaEscapes.Contains(ch))
				sb.Append(ch);
		}
		sb.Append(@""";");

		sb.AppendLine();
		sb.Append(@"UnescapedAnsiCharSet = @""");
		for (var i = 32; i <= 126; i++) {
			var ch = Encoding.ASCII.GetString(new byte[] { (byte)i }).ToString();

			if (csharpEscapes.Contains(ch))
				ch = @"""";
			if (!epasaEscapes.Contains(ch))
				sb.Append(ch);
		}
		sb.Append(@""";");


		sb.AppendLine();
		sb.Append(@"FullAnsiCharSet = @""");
		for (var i = 32; i <= 126; i++) {
			var ch = Encoding.ASCII.GetString(new byte[] { (byte)i }).ToString();
			if (csharpEscapes.Contains(ch))
				ch = @"""";
			sb.Append(ch);
		}
		sb.Append(@""";");
		_standardTextBox.Clear();
		_standardTextBox.AppendText(sb.ToString());
	}

	private void _genOnlyButton_Click(object sender, EventArgs e) {
		try {
			_standardTextBox.Clear();
			var start = DateTime.Now;
			long genSize = 0;
			while (genSize < 1000000) {
				var randomString = Tools.Text.GenerateRandomString(50) + Environment.NewLine;
				genSize += randomString.Length * sizeof(char);
			}
			DialogEx.Show(this, SystemIconType.Information, "Results", "{0} bytes took {1} seconds".FormatWith(genSize, DateTime.Now.Subtract(start).TotalSeconds));
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}

	private void _fillLockedAsyncButton_Click(object sender, EventArgs e) {
		try {
			//_standardTextBox.Clear();
			//var start = DateTime.Now;
			//long genSize = 0;
			//if (!WinAPI.USER32.LockWindowUpdate(_lockTextBox.Handle)) {
			//    throw new Exception("Unable to lock");
			//}
			//while (genSize < 1000000) {                    
			//    var randomString = Tools.Text.GenerateRandomString(50) + Environment.NewLine;
			//    genSize += randomString.Length * sizeof(char);
			//    _lockTextBox.AppendText(randomString);
			//}
			//if (!WinAPI.USER32.LockWindowUpdate(IntPtr.Zero)) {
			//    throw new Exception("Unable to unlock");
			//}

			//DialogEx.Show(this, SystemIconType.Information, "Results", "{0} bytes took {1} seconds".FormatWith(genSize, DateTime.Now.Subtract(start).TotalSeconds));
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}

	public const string Pascal64CharSet = "abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-+{}[]_:`|<>,.?/~";
	public const string Pascal64StartCharSet = "abcdefghijklmnopqrstuvwxyz!@#$%^&*()-+{}[]_:`|<>,.?/~";
	public const string HexStringCharSet = "0123456789abcdef";
	public const string Base58CharSet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
	public const string SafeUnescapedAnsiCharSet = "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[]^_`abcdefghijklmnopqrstuvwxyz{|~";
	public string SafeEscapedAnsiCharSet = "\"\\}";
	public const char EscapeChar = '\\';


	private void _epasaRegexButton_Click(object sender, EventArgs e) {

		try {
			//var unescapedSafeAnsiCharPattern =
			//	RegexPattern.With.Set(RegexPattern.With.Literal(SafeUnescapedAnsiCharSet));

			var unescapedSafeAnsiCharPatterns = SafeUnescapedAnsiCharSet
				.Select(c => RegexPattern.With.Literal(c.ToString()));

			var escapedSafeAnsiCharPatterns = SafeEscapedAnsiCharSet
				.Select(c => RegexPattern.With.Literal("\\").Literal(c.ToString()));

			var safeAnsiCharPattern = RegexPattern.With.Choice.EitherAny(
				unescapedSafeAnsiCharPatterns.Concat(escapedSafeAnsiCharPatterns).ToArray()
			);


			var safeAnsiStringPattern = new RegexPattern(safeAnsiCharPattern)
				.Repeat.OneOrMore;

			var pascal64StartPattern = RegexPattern.With
				.Set(RegexPattern.With.Literal(Pascal64StartCharSet, true));

			// account name needs to omit brackets!

			var pascal64StringPattern =
				new RegexPattern(pascal64StartPattern)
					.Set(RegexPattern.With.Literal(Pascal64CharSet, true)).Repeat.InRange(2, 63);


			var hexNibblePattern = RegexPattern.With
				.Set(RegexRange.OfMany(Tuple.Create('0', '9'), Tuple.Create('a', 'f')));

			var hexBytePattern = RegexPattern.With
				.Group(
					new RegexPattern(hexNibblePattern).Repeat.Exactly(2)
				);

			var hexStringPattern = new RegexPattern(hexBytePattern).Repeat.OneOrMore;

			var base58CharPattern = RegexPattern.With
				.Set(RegexPattern.With.Literal(Base58CharSet));

			var base58StringPattern = new RegexPattern(base58CharPattern).Repeat.OneOrMore;

			var integerPattern = RegexPattern
				.With.Digit.Repeat.OneOrMore;

			var accountNamePattern = RegexPattern
				.With.NamedGroup("AccountName", pascal64StringPattern);


			var accountNumberPattern = RegexPattern
				.With.NamedGroup("AccountNumber", integerPattern)
				.Literal("-")
				.NamedGroup("Checksum", RegexPattern.With.Digit.Repeat.Exactly(2));

			var pasaPattern = RegexPattern
				.With.Choice.Either(accountNamePattern, accountNumberPattern);

			var payloadPattern = RegexPattern.With
				.Choice.EitherAny(
					RegexPattern.With.NamedGroup("ASCIIPayload", RegexPattern.With.Literal("\"").Group(safeAnsiStringPattern).Literal("\"")).Repeat.Optional,
					RegexPattern.With.NamedGroup("HexPayload", RegexPattern.With.Literal("0x").Group(hexStringPattern)).Repeat.Optional,
					RegexPattern.With.NamedGroup("Base58Payload", base58StringPattern).Repeat.Optional
				);

			var publicPayloadPattern = RegexPattern.With
				.Literal("[")
				.NamedGroup("NoEncryption", payloadPattern).Repeat.Optional
				.Literal("]", true);

			var receiverPayloadPattern = RegexPattern.With
				.Literal("(")
				.NamedGroup("ReceiverEncrypted", payloadPattern).Repeat.Optional
				.Literal(")");

			var senderPayloadPattern = RegexPattern.With
				.Literal("<")
				.NamedGroup("SenderEncrypted", payloadPattern).Repeat.Optional
				.Literal(">");

			var aesPayloadPattern = RegexPattern.With
				.Literal("<")
				.NamedGroup("AESEncrypted", payloadPattern).Repeat.Optional
				.Literal(">")
				.Literal(":")
				.NamedGroup("Password", safeAnsiStringPattern);

			var extendedAddressPattern = RegexPattern.With
				.Choice.EitherAny(
					publicPayloadPattern,
					receiverPayloadPattern,
					senderPayloadPattern,
					aesPayloadPattern
				);

			var epasaChecksumPattern = new RegexPattern(hexBytePattern)
				.Repeat.Exactly(2);

			var epasaPattern = RegexPattern
				.With.AtBeginning
				.NamedGroup("PASA", pasaPattern)
				.RegEx(new RegexPattern(extendedAddressPattern)).Repeat.Optional
				.Group(
					RegexPattern.With
						.Literal(":")
						.NamedGroup("EPASAChecksum", epasaChecksumPattern)
						.AtEnd
				).Repeat.Optional;


			_standardTextBox.Clear();
			_standardTextBox.AppendText(epasaPattern.ToString());

			Match subregex;
			{
				//subregex = new Regex(unescapedSafeAnsiCharPattern, RegexOptions.None).Match("77-44[0x1234]:121f");
				_standardTextBox.AppendLine("const string safeAnsiCharPattern = \"" + safeAnsiCharPattern + "\"");
				_standardTextBox.AppendLine("const string safeAnsiStringPattern = \"" + safeAnsiStringPattern + "\"");
				_standardTextBox.AppendLine("const string pascal64StartPattern = \"" + pascal64StartPattern + "\"");
				_standardTextBox.AppendLine("const string pascal64StringPattern = \"" + pascal64StringPattern + "\"");
				_standardTextBox.AppendLine("const string hexNibblePattern = \"" + hexNibblePattern + "\"");
				_standardTextBox.AppendLine("const string hexBytePattern = \"" + hexBytePattern + "\"");
				_standardTextBox.AppendLine("const string hexStringPattern = \"" + hexStringPattern + "\"");
				_standardTextBox.AppendLine("const string base58CharPattern = \"" + base58CharPattern + "\"");
				_standardTextBox.AppendLine("const string base58StringPattern = \"" + base58StringPattern + "\"");
				_standardTextBox.AppendLine("const string integerPattern = \"" + integerPattern + "\"");
				_standardTextBox.AppendLine("const string accountNamePattern = \"" + accountNamePattern + "\"");
				_standardTextBox.AppendLine("const string accountNumberPattern = \"" + accountNumberPattern + "\"");
				_standardTextBox.AppendLine("const string pasaPattern = \"" + pasaPattern + "\"");
				_standardTextBox.AppendLine("const string payloadPattern = \"" + payloadPattern + "\"");
				_standardTextBox.AppendLine("const string publicPayloadPattern = \"" + publicPayloadPattern + "\"");
				_standardTextBox.AppendLine("const string receiverPayloadPattern = \"" + receiverPayloadPattern + "\"");
				_standardTextBox.AppendLine("const string senderPayloadPattern = \"" + senderPayloadPattern + "\"");
				_standardTextBox.AppendLine("const string aesPayloadPattern = \"" + aesPayloadPattern + "\"");
				_standardTextBox.AppendLine("const string extendedAddressPattern = \"" + extendedAddressPattern + "\"");
				_standardTextBox.AppendLine("const string epasaChecksumPattern = \"" + epasaChecksumPattern + "\"");
				_standardTextBox.AppendLine("const string epasaPattern = \"" + epasaPattern + "\"");
			}

			_standardTextBox.AppendLine("");



			var regex = new Regex(epasaPattern, RegexOptions.None);

			var result = regex.Match("77-44[0x123]:121f");
			result = regex.Match("77-44[0x123]");
			result = regex.Match("77-44[B58xdf]:121f");
			result = regex.Match("77-44");
			result = regex.Match("account-name");
			result = regex.Match("account-name:abcd");
			_standardTextBox.AppendLine(result.ToString());
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}
}
