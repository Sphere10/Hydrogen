// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public interface IItemSerializerDecorator {
	internal IItemSerializer InternalSerializer { get; }
}

public class ItemSerializerDecorator<TItem, TSerializer> : ItemSizerDecorator<TItem, TSerializer>, IItemSerializer<TItem>, IItemSerializerDecorator
	where TSerializer : IItemSerializer<TItem> {

	public ItemSerializerDecorator(TSerializer internalSerializer)
		: base(internalSerializer) {
	}

	internal ItemSerializerDecorator()
		: base() {
	}

	public virtual void Serialize(TItem item, EndianBinaryWriter writer, SerializationContext context)
		=> Internal.Serialize(item, writer, context);

	public virtual TItem Deserialize(EndianBinaryReader reader, SerializationContext context)
		=> Internal.Deserialize(reader, context);

	public IItemSerializer InternalSerializer => base.Internal;
}

public class ItemSerializerDecorator<TItem> : ItemSerializerDecorator<TItem, IItemSerializer<TItem>> {

	public ItemSerializerDecorator(IItemSerializer<TItem> internalSerializer)
		: base(internalSerializer) {
	}

	internal ItemSerializerDecorator()
		: base() {
	}

}
