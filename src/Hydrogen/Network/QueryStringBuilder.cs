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

namespace Hydrogen;

public class QueryStringBuilder {
	private readonly List<KeyValuePair<string, string>> _list;

	public QueryStringBuilder() {
		_list = new List<KeyValuePair<string, string>>();
	}

	public void Add<T>(string name, T value) {
		Add(new KeyValuePair<string, object>(name, value?.ToString()));
	}

	public void Add<T>(params KeyValuePair<string, T>[] @params) {
		if (@params.Length > 0) {
			foreach (var param in @params)
				_list.Add(new KeyValuePair<string, string>(param.Key, param.Value?.ToString() ?? string.Empty));
		}
	}

	public IEnumerable<KeyValuePair<string, string>> Parameters => _list.Select(x => x);

	public override string ToString() {
		return ToString(string.Empty);
	}

	public string ToString(string baseUrl) {
		baseUrl = baseUrl ?? string.Empty;
		var queryString = _list.Any() ? string.Join("&", _list.Select(kvp => string.Concat(Uri.EscapeDataString(kvp.Key), "=", Uri.EscapeDataString(kvp.Value.ToString())))) : string.Empty;
		return baseUrl + (queryString.Length > 0 ? "?" + queryString : "");
	}

	public static string BuildFrom<T>(string baseUrl = null, params KeyValuePair<string, T>[] parameters) {
		var builder = new QueryStringBuilder();
		foreach (var param in (parameters ?? new KeyValuePair<string, T>[0]))
			builder.Add(param);
		return builder.ToString(baseUrl);
	}
}
