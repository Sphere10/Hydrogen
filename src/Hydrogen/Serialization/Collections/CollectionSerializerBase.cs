// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public abstract class CollectionSerializerBase<TCollection, TItem> : ItemSerializerBase<TCollection> where TCollection : IEnumerable {
	private readonly SizeDescriptorSerializer _sizeSerializer;
	private readonly IItemSerializer<TItem> _itemSerializer;

	protected CollectionSerializerBase(IItemSerializer<TItem> itemSerializer, SizeDescriptorStrategy sizeDescriptorStrategy) {
		Guard.ArgumentNotNull(itemSerializer, nameof(itemSerializer));
		_itemSerializer = itemSerializer;
		_sizeSerializer = new SizeDescriptorSerializer(sizeDescriptorStrategy);
	}

	public override long CalculateSize(SerializationContext context, TCollection collection) {
		var sizeSize = _sizeSerializer.CalculateSize(context, GetLength(collection));
		
		var itemsSize = 0L;
		foreach (var item in collection)
			itemsSize += _itemSerializer.CalculateSize(context, (TItem)item);

		return sizeSize + itemsSize;
	}

	public override void Serialize(TCollection collection, EndianBinaryWriter writer, SerializationContext context) {
		var asArray = collection switch {
			TItem[] arr => arr,
			IEnumerable<TItem> enumerable => enumerable.ToArray(),
			_ => collection.Cast<TItem>().ToArray()
		};
		_sizeSerializer.Serialize(asArray.Length, writer, context);
		foreach (var element in asArray)
			_itemSerializer.Serialize(element, writer, context);
	}

	public override TCollection Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var length = _sizeSerializer.Deserialize(reader, context);
		var collection = Activate(length);
		context.SetDeserializingItem(collection);
		for (var i = 0L; i < length; i++) {
			var item = _itemSerializer.Deserialize(reader, context);
			SetItem(collection, i, item);
		}
		return collection;
	}


	protected abstract long GetLength(TCollection collection);

	protected abstract TCollection Activate(long capacity);
	
	protected abstract void SetItem(TCollection collection, long index, TItem item);
}
