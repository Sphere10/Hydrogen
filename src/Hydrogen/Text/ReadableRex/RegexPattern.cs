// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Text;

namespace Hydrogen;

public class RegexPattern {
	readonly StringBuilder _content;

	/// <summary>
	/// Indicates creation of a new pattern
	/// </summary>
	public static RegexPattern With => new RegexPattern(string.Empty);

	public RegexPattern(string content) {
		//Do we even need this public? Should we force everyone to use Pattern.With syntax?
		if (content == null) throw new ArgumentNullException(nameof(content));
		_content = new StringBuilder(content);
	}

	private string Eval() {
		return _content.ToString();
	}

	/// <summary>
	/// A string that will be properly escaped so that reserved characters are treated as literals
	/// </summary>
	/// <param name="content"></param>
	/// <returns></returns>
	public RegexPattern Literal(string content, bool insideSet = false) {
		const string reservedCharactersStandard = @".$^{[(|)*+?\";
		const string reservedCharactersInsideSet = @".$^{[](|)*+?\";
		var reservedCharacters = insideSet ? reservedCharactersInsideSet : reservedCharactersStandard;
		foreach (var character in content) {
			if (reservedCharacters.IndexOf(character) < 0) {
				_content.Append(character);
			} else {
				_content.Append('\\').Append(character);
			}
		}
		return this;
	}

	/// <summary>
	/// Any existing regular expression pattern.
	/// </summary>
	/// <param name="pattern"></param>
	/// <returns></returns>
	public RegexPattern RegEx(string pattern) {
		_content.Append(pattern);
		return this;
	}

	public RegexPattern Anything {
		get {
			_content.Append(@".");
			return this;
		}
	}

	public RegexPattern Word {
		get {
			_content.Append(@"\w");
			return this;
		}
	}

	public RegexPattern Digit {
		get {
			_content.Append(@"\d");
			return this;
		}
	}

	public RegexPattern WhiteSpace {
		get {
			_content.Append(@"\s");
			return this;
		}
	}

	public override string ToString() {
		return Eval();
	}

	public RegexPattern Clone() {
		return new RegexPattern(ToString());
	}

	public RegexPattern NonWord {
		get {
			_content.Append(@"\W");
			return this;
		}
	}

	public RegexPattern NonDigit {
		get {
			_content.Append(@"\D");
			return this;
		}
	}

	public RegexPattern NonWhiteSpace {
		get {
			_content.Append(@"\S");
			return this;
		}
	}

	/// <summary>
	/// A subset of the pattern that can be referenced as ordinal captures
	/// </summary>
	/// <param name="innerExpression"></param>
	/// <returns></returns>
	public RegexPattern Group(RegexPattern innerExpression) {
		_content.Append($"({innerExpression})");
		return this;
	}

	/// <summary>
	/// A subset of the pattern that can be referenced as a named capture
	/// </summary>
	/// <param name="groupName"></param>
	/// <param name="innerExpression"></param>
	/// <returns></returns>
	public RegexPattern NamedGroup(string groupName, RegexPattern innerExpression) {
		_content.AppendFormat("(?<{1}>{0})", innerExpression, groupName);
		return this;
	}

	/// <summary>
	/// A non-capturing group
	/// </summary>
	/// <param name="innerExpression"></param>
	/// <returns></returns>
	public RegexPattern Phrase(RegexPattern innerExpression) {
		_content.Append($"(?:{innerExpression})");
		return this;
	}

	/// <summary>
	/// Matches any single character contained within
	/// </summary>
	/// <param name="innerExpression"></param>
	/// <returns></returns>
	public RegexPattern Set(RegexPattern innerExpression) {
		_content.Append($"[{innerExpression}]");
		return this;
	}

	/// <summary>
	/// Matches any single character not contained within
	/// </summary>
	/// <param name="innerExpression"></param>
	/// <returns></returns>
	public RegexPattern NegatedSet(RegexPattern innerExpression) {
		_content.Append($"[^{innerExpression}]");
		return this;
	}

	/// <summary>
	/// Quantifies how many times to detect the previous element
	/// </summary>
	public RegexQuantifier Repeat => new RegexQuantifier(this);

	/// <summary>
	/// Specifies that the match must occur at the beginning of the string or the beginning of the line
	/// </summary>
	public RegexPattern AtBeginning {
		get {
			_content.Append('^');
			return this;
		}
	}

	/// <summary>
	/// Specifies that the match must occur at the end of the string, before \n at the end of the string, or at the end of the line.
	/// </summary>
	public RegexPattern AtEnd {
		get {
			_content.Append('$');
			return this;
		}
	}

	public static implicit operator string(RegexPattern expression) {
		return expression.ToString();
	}

	public static RegexPattern operator +(RegexPattern first, RegexPattern second) {
		first._content.Append(second.ToString());
		return first;
	}

	public RegexAlternation Choice => new RegexAlternation(this);
}
