// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class AutoSizedSerializer<TItem> : ItemSerializerDecorator<TItem> {

	public AutoSizedSerializer(IItemSerializer<TItem> internalSerializer)
		: base(internalSerializer) {
	}

	public override int CalculateSize(TItem item) => sizeof(int) + base.CalculateSize(item);

	public bool TrySerialize(TItem item, EndianBinaryWriter writer)
		=> TrySerialize(item, writer, out _);

	public override bool TrySerialize(TItem item, EndianBinaryWriter writer, out int bytesWritten) {
		var len = item != null ? base.CalculateSize(item) : 0;
		writer.Write(len);
		var res = base.TrySerialize(item, writer, out bytesWritten);
		bytesWritten += sizeof(int);
		return res;
	}

	public override bool TryDeserialize(int byteSize, EndianBinaryReader reader, out TItem item) {
		// Caller trying to read item passing explicit size, so check length matches
		var len = reader.ReadInt32();
		Guard.ArgumentEquals(byteSize, sizeof(int) + len, nameof(byteSize), "Read overflow");
		return base.TryDeserialize(len, reader, out item);
	}

	public bool TryDeserialize(EndianBinaryReader reader, out TItem item) {
		var len = reader.ReadInt32();
		return base.TryDeserialize(len, reader, out item);
	}

	public TItem Deserialize(EndianBinaryReader reader) {
		if (!TryDeserialize(reader, out var item))
			throw new InvalidOperationException($"Unable to deserialize object");
		return item;
	}

}
