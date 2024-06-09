// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Converts an IList to IExtendedList. 
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class ExtendedListAdapter<TItem> : SingularListBase<TItem> {

	private readonly IList<TItem> _endpoint;

	public ExtendedListAdapter()
		: this(new List<TItem>()) {
	}

	public ExtendedListAdapter(IList<TItem> endpoint) {
		_endpoint = endpoint;
	}

	public override long Count => _endpoint.Count;

	public override bool IsReadOnly => _endpoint.IsReadOnly;

	public override long IndexOfL(TItem item) => _endpoint.IndexOf(item);

	public override bool Contains(TItem item) => _endpoint.Contains(item);

	public override IEnumerable<bool> ContainsRange(IEnumerable<TItem> items) => EnsureSafe(items).Select(Contains).ToArray();

	public override TItem Read(long index) {
		checked {
			return _endpoint[(int)EnsureSafe(index)];
		}
	}

	public override void Add(TItem item) => _endpoint.Add(item);

	public override void Update(long index, TItem item) {
		checked {
			_endpoint[(int)EnsureSafe(index)] = item;
		}
	}

	public override void Insert(long index, TItem item) {
		checked {
			_endpoint.Insert((int)EnsureSafe(index, allowAtEnd: true), item);
		}
	}

	public override void RemoveAt(long index) {
		checked {
			_endpoint.RemoveAt((int)EnsureSafe(index));
		}
	}

	public override void Clear() => _endpoint.Clear();

	public override bool Remove(TItem item) => _endpoint.Remove(item);

	public override void CopyTo(TItem[] array, int arrayIndex) => _endpoint.CopyTo(array, arrayIndex);

	public override IEnumerator<TItem> GetEnumerator() => _endpoint.GetEnumerator();

}
