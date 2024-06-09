// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Diagnostics;

namespace Hydrogen;

public class TextBank {
	private List<string> _data;

	public TextBank() {
		_data = new List<string>();
	}

	public TextBank(IEnumerable<object> collection) {
		AddObjects(collection);
	}


	public List<string> Data {
		get { return _data; }
		set { _data = value; }
	}

	public void AddObjects(IEnumerable<object> array) {
		Debug.Assert(array != null);
		foreach (object obj in array) {
			_data.Add(obj.ToString());
		}
	}

	public string[] SearchText(string subStringToMatch) {
		Debug.Assert(subStringToMatch != null);
		return SearchText(subStringToMatch, false);
	}

	public string[] SearchText(string subStringToMatch, bool caseSensitive) {
		Debug.Assert(subStringToMatch != null);
		List<string> res = new List<string>();
		foreach (string str in _data) {
			string strToUse = caseSensitive ? str.ToUpper() : str;
			string strToFind = caseSensitive ? subStringToMatch.ToUpper() : subStringToMatch;
			if (strToUse.Contains(strToFind)) {
				res.Add(str);
			}
		}
		return res.ToArray();
	}

	public bool ContainsText(string subStringToMatch) {
		Debug.Assert(subStringToMatch != null);
		return ContainsText(subStringToMatch, false);
	}

	public bool ContainsText(string subStringToMatch, bool caseSensitive) {
		Debug.Assert(subStringToMatch != null);
		bool found = false;
		foreach (string str in _data) {
			string strToUse = caseSensitive ? str.ToUpper() : str;
			string strToFind = caseSensitive ? subStringToMatch.ToUpper() : subStringToMatch;
			if (strToUse.Contains(strToFind)) {
				found = true;
				break;
			}
		}
		return found;
	}


}
