// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.DApp.Node.UI;

public class ListDataSource<T> {
	public const int MinNameColumnWidth = 5;
	public const int MaxNameColumnWidth = 50;

	private readonly List<string> _itemLabels;
	private readonly List<T> _items;

	private readonly Func<T, Tuple<string, string>> _describeFunc;
	private readonly int _nameColumnWidth;

	public ListDataSource(IEnumerable<T> items, Func<T, string> nameAction, Func<T, string> descriptionAction)
		: this(items, (x) => Tuple.Create(nameAction(x), descriptionAction(x))) {
	}

	public ListDataSource(IEnumerable<T> items, Func<T, Tuple<string, string>> describeFunc) {
		Guard.ArgumentNotNull(items, nameof(items));
		Guard.ArgumentNotNull(describeFunc, nameof(describeFunc));
		_items = items.ToList();
		_describeFunc = describeFunc;
		_itemLabels = ToLabels();
		MaxLen = _itemLabels.Max(x => x.Length);
	}

	public int Count => _items.Count;

	public IReadOnlyList<T> Items => _items;

	public IList Labels => _itemLabels;

	public int MaxLen { get; }

	private List<string> ToLabels() {
		var itemDescriptions = _items.Select(_describeFunc).ToArray();
		var maxNameLen = itemDescriptions.Max(x => x.Item1.Length);
		var maxDescLen = itemDescriptions.Max(x => x.Item2.Length);

		var list = new List<string>();
		foreach (var item in itemDescriptions) {
			var paddedName = item.Item1.PadRight(maxNameLen);
			var paddedDesc = !string.IsNullOrWhiteSpace(item.Item2) ? $"    {item.Item2}" : string.Empty;
			var itemDesc = $"{paddedName}{paddedDesc}";
			list.Add(itemDesc);
		}
		return list;
	}


}
