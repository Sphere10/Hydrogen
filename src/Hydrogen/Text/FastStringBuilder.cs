//-----------------------------------------------------------------------
// <copyright file="FastStringBuilder.cs" company="Sphere 10 Software">
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
using System.Text;

namespace Sphere10.Framework {
	public class FastStringBuilder {
		private const int DefaultCapacity = 1000;
		private readonly List<Tuple<int, string>> _strings;

		public FastStringBuilder() : this(DefaultCapacity) {
		}

		public FastStringBuilder(int estimatedNumberOfStrings) {
			_strings = new List<Tuple<int, string>>(estimatedNumberOfStrings);

		}

		public IEnumerable<string> Strings => _strings.Select(x => x.Item2);

		public void Append(string str) {
			_strings.Add(Tuple.Create(Length + str.Length, str));
		}

		public void AppendFormat(string str, params object[] formatParams) {
			Append(string.Format(str, formatParams));
		}

		public void AppendLine(string str, params object[] formatParams) {
			AppendFormat(str + Environment.NewLine, formatParams);
		}

		public int Length {
			get {
				var numStrings = _strings.Count;
				return numStrings == 0 ? 0 : _strings[numStrings - 1].Item1; ;
			}
		}

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
				choppedOffPart.Append( new string( lastRemovedItem.Item2.Reverse().ToArray()));
			} while (amountChopped < lengthToChop && itemsRemoved < itemCount);

			_strings.RemoveRange(itemCount - itemsRemoved, itemsRemoved);

			if (amountChopped > lengthToChop) {
				var amountToRestore = amountChopped - lengthToChop;
				Append(lastRemovedItem.Item2.Substring( lastRemovedItem.Item2.Length - amountToRestore, lastRemovedItem.Item2.Length));
				choppedOffPart.ChopFromEnd(amountToRestore);
			}

			return new string (choppedOffPart.ToString().Reverse().ToArray());
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
}

