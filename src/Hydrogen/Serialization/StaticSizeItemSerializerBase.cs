// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public abstract class StaticSizeItemSerializerBase<TItem> : StaticSizeItemSizer<TItem>, IItemSerializer<TItem> {
	protected StaticSizeItemSerializerBase(int fixedSize) : base(fixedSize) {
	}

	public bool TrySerialize(TItem item, EndianBinaryWriter writer, out int bytesWritten) {
		var result = TrySerialize(item, writer);
		bytesWritten = result ? StaticSize : 0;
		return result;
	}

	public abstract bool TrySerialize(TItem item, EndianBinaryWriter writer);

	public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out TItem item)
		=> TryDeserialize(reader, out item);

	public abstract bool TryDeserialize(EndianBinaryReader reader, out TItem item);


	public TItem Deserialize(EndianBinaryReader reader) {
		if (!TryDeserialize(reader, out var item))
			throw new InvalidOperationException($"Unable to deserialize object");
		return item;
	}
}
