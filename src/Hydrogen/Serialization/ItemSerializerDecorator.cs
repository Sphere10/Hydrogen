// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class ItemSerializerDecorator<TItem, TSerializer> : ItemSizerDecorator<TItem, TSerializer>, IItemSerializer<TItem>
	where TSerializer : IItemSerializer<TItem> {

	public ItemSerializerDecorator(TSerializer internalSerializer)
		: base(internalSerializer) {
	}

	public virtual void SerializeInternal(TItem item, EndianBinaryWriter writer)
		=> Internal.Serialize(item, writer);

	public virtual TItem DeserializeInternal(long byteSize, EndianBinaryReader reader)
		=> Internal.Deserialize(byteSize, reader);

}

public class ItemSerializerDecorator<TItem> : ItemSerializerDecorator<TItem, IItemSerializer<TItem>> {
	public ItemSerializerDecorator(IItemSerializer<TItem> internalSerializer)
		: base(internalSerializer) {
	}

}
