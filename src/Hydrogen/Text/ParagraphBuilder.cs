// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


using System;
using System.Linq;
using System.Text;

namespace Hydrogen;

public class ParagraphBuilder {
	public static readonly string ParagraphBreak;
	public static readonly char[] SentenceFinalizers;
	public static readonly char DefaultSentenceFinalizer;

	static ParagraphBuilder() {
		ParagraphBreak = Environment.NewLine + Environment.NewLine;
		SentenceFinalizers = new char[] { '.', '!', '?', ';' };
		DefaultSentenceFinalizer = '.';
	}

	private StringBuilder _builder;
	private String _lastAppendedText;

	public ParagraphBuilder() {
		_builder = new StringBuilder();
		_lastAppendedText = string.Empty;
	}

	public int Length {
		get { return _builder.Length; }
	}

	public void AppendSentence(string text) {
		AppendSentence(text, true);
	}

	public void AppendSentence(string text, bool format) {
		text = text.Trim();
		if (!string.IsNullOrEmpty(text)) {
			Append(format ? text.ToSentenceCase() : text);
		}
	}

	public void AppendSentences(params string[] strings) {
		strings.ForEach(s => AppendSentence(s));
	}

	public void AppendParagraphBreak() {
		if (!_lastAppendedText.EndsWith(ParagraphBreak)) {
			Append(ParagraphBreak);
		}
	}

	public void Clear() {
		_builder.Remove(0, _builder.Length);
	}


	public static string Combine(params string[] sentences) {
		ParagraphBuilder builder = new ParagraphBuilder();
		builder.AppendSentences(sentences);
		return builder.ToString();
	}

	private void Append(string text) {
		var textToAppend = new StringBuilder();
		textToAppend.Append(text);
		if (_lastAppendedText.Length > 0 && _lastAppendedText.Last().IsIn(SentenceFinalizers)) {
			textToAppend.Insert(0, " ");
		}
		_builder.Append(textToAppend);
		_lastAppendedText = textToAppend.ToString();
	}

	public override string ToString() {
		return _builder.ToString();
	}


	public static string StringToSentenceCase(string value) {
		var str = value.Trim();
		if (str.Length > 1 && !char.IsUpper(str[0])) {
			str = str.Substring(0, 1).ToUpper() + str.Substring(1);
		}
		if (str.Length > 0 && !str.Last().IsIn(SentenceFinalizers)) {
			str += DefaultSentenceFinalizer;
		}
		return str;
	}
}
