// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Hydrogen;

public sealed class BinarySerializedPage<TItem> : FileSwappedMemoryPage<TItem> {

	public BinarySerializedPage(long pageSize, IItemSizer<TItem> sizer)
		: base(pageSize, sizer, new ExtendedList<TItem>()) {
	}

	protected override void SaveInternal(IExtendedList<TItem> memoryPage, Stream stream) {
		var formatter = new BinarySerializer();
		var itemsArr = memoryPage as TItem[] ?? memoryPage.ToArray();
		formatter.Serialize(stream, itemsArr);
		stream.SetLength(Math.Max(0, stream.Position)); // end stream after serialization
	}

	protected override void LoadInternal(Stream stream, IExtendedList<TItem> memoryPage) {
		TItem[] result;
		if (stream.Length > 0) {
			var formatter = new BinarySerializer();
			result = (TItem[])formatter.Deserialize(stream);
		} else {
			result = new TItem[0];
		}
		memoryPage.AddRange(result);
		Debug.Assert(result.Count() == Count);
	}
}
