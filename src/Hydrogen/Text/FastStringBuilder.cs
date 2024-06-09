// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydrogen;

public class FastStringBuilder {
	private const int DefaultCapacity = 1000;
	private readonly List<Tuple<int, string>> _strings;

	public FastStringBuilder() : this(DefaultCapacity) {
	}

	public FastStringBuilder(int estimatedNumberOfStrings) {
		_strings = new List<Tuple<int, string>>(estimatedNumberOfStrings);

	}

	public IEnumerable<string> Strings => _strings.Select(x => x.Item2);

	public void Prepend(string str) {
		Insert(0, str);
	}

	public void PrependFormat(string str, params object[] formatParams) {
		Prepend(string.Format(str, formatParams));
	}

	public void PrependLine(string str, params object[] formatParams) {
		PrependFormat(str + Environment.NewLine, formatParams);
	}

	public void Append(string str) {
		Insert(_strings.Count, str);
	}

	public void AppendFormat(string str, params object[] formatParams) {
		Append(string.Format(str, formatParams));
	}

	public void AppendLine(string str, params object[] formatParams) {
		AppendFormat(str + Environment.NewLine, formatParams);
	}


	public void Insert(int index, string str) {
		var subTotal = index == 0 ? 0 : index == _strings.Count ? Length : _strings.Take(index).Sum(x => x.Item1);
		subTotal += str.Length;
		_strings.Insert(index, Tuple.Create(subTotal, str));
		for (var i = index + 1; i < _strings.Count; i++) {
			var item = _strings[i];
			subTotal += _strings[i].Item1;
			_strings[i] = Tuple.Create(subTotal, item.Item2);
		}
	}

	public int Length {
		get {
			var numStrings = _strings.Count;
			return numStrings == 0 ? 0 : _strings[numStrings - 1].Item1;
			;
		}
	}

	public int SubStringCount => _strings.Count;

	public string ChopFromEnd(int lengthToChop) {
		var itemCount = _strings.Count;

		if (itemCount == 0)
			return string.Empty;

		var amountChopped = 0;
		var itemsRemoved = 0;
		Tuple<int, string> lastRemovedItem;
		var choppedOffPart = new FastStringBuilder();
		do {
			lastRemovedItem = _strings[itemCount - itemsRemoved - 1];
			amountChopped += lastRemovedItem.Item2.Length;
			itemsRemoved++;
			choppedOffPart.Prepend(lastRemovedItem.Item2);
		} while (amountChopped < lengthToChop && itemsRemoved < itemCount);

		_strings.RemoveRange(itemCount - itemsRemoved, itemsRemoved);

		var amountToRestore = 0;
		if (amountChopped > lengthToChop) {
			amountToRestore = amountChopped - lengthToChop;
			Append(lastRemovedItem.Item2.Substring(0, amountToRestore));
		}

		return choppedOffPart.ToString().Substring(amountToRestore);
	}

	public void Clear() {
		_strings.Clear();
	}

	public static string From(IEnumerable<string> parts, bool appendNewLine = false) {
		var partsArr = parts as string[] ?? parts.ToArray();
		var builder = new FastStringBuilder(partsArr.Length);
		if (appendNewLine)
			partsArr.ForEach(s => builder.AppendLine(s));
		else
			partsArr.ForEach(builder.Append);

		return builder.ToString();
	}

	public override string ToString() {
		var stringBuilder = new StringBuilder(this.Length);
		foreach (var item in _strings)
			stringBuilder.Append(item.Item2);
		return stringBuilder.ToString();
	}
}
