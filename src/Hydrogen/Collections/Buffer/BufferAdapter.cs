﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class BufferAdapter : ExtendedListAdapter<byte>, IBuffer {

	public BufferAdapter()
		: this(new List<byte>()) {
	}

	public BufferAdapter(IList<byte> endpoint)
		: base(endpoint) {
	}

	public ReadOnlySpan<byte> ReadSpan(long index, long count) {
		return base.ReadRange(index, count).ToArray();
	}

	public void AddRange(ReadOnlySpan<byte> span) {
		base.AddRange((IEnumerable<byte>)span.ToArray());
	}

	public void UpdateRange(long index, ReadOnlySpan<byte> items) {
		base.UpdateRange(index, (IEnumerable<byte>)items.ToArray());
	}

	public void InsertRange(long index, ReadOnlySpan<byte> items) {
		base.InsertRange(index, (IEnumerable<byte>)items.ToArray());
	}

	public Span<byte> AsSpan(long index, long count) {
		throw new NotSupportedException();
	}

	public void ExpandTo(long totalBytes) {
		var newBytes = totalBytes - base.Count;
		if (newBytes > 0)
			ExpandBy(newBytes);
	}

	public void ExpandBy(long newBytes) {
		for (var i = 0; i < newBytes; i++)
			base.Add(default);
	}
}
