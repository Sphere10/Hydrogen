// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Hydrogen;

/// <summary>
/// Serializes a <see cref="KeyValuePair{TKey, TValue}"/> using component serializers for <see cref="TKey"/> and <see cref="TValue"/>. 
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <remarks>Serialization format: [KeySize] [KeyBytes....] [ValueSize] [ValueBytes....]
///  - A key-size is always serialized so it can be skipped when only reading values.
///  - Null values are supported (even if value serializer doesn't)
///  - Null keys are not supported by default, but passing a key serializer that supports null will
///  - Since keys/values are accessible independently, they use their own context thus cannot share references from previous context. This needs to be addressed later.
/// </remarks>
public class KeyValuePairSerializer<TKey, TValue> : ItemSerializer<KeyValuePair<TKey, TValue>> {

	public KeyValuePairSerializer(IItemSerializer<TKey> keySerializer = null, IItemSerializer<TValue> valueSerializer = null, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt)
		: base(sizeDescriptorStrategy) {
		KeySerializer = keySerializer ?? ItemSerializer<TKey>.Default;
		ValueSerializer = (valueSerializer ?? ItemSerializer<TValue>.Default).AsSanitized();
	}

	public IItemSerializer<TKey> KeySerializer { get; }

	public IItemSerializer<TValue> ValueSerializer { get; }

	public override long CalculateSize(SerializationContext context, KeyValuePair<TKey, TValue> item) {
		var keySize = KeySerializer.CalculateSize(context, item.Key);
		var keySizeDescriptorSize = SizeSerializer.CalculateSize(context, keySize);
		var valueSize = ValueSerializer.CalculateSize(context, item.Value);
		var valueSizeDescriptorSize = SizeSerializer.CalculateSize(context, valueSize);
		return keySizeDescriptorSize + keySize + valueSizeDescriptorSize + valueSize;
	}

	public override void Serialize(KeyValuePair<TKey, TValue> item, EndianBinaryWriter writer, SerializationContext context) {
		// NOTE: since keys/values are accessible independently, we must use a fresh context for each

		// key value		
		using (var keyContext = SerializationContext.New) {
			var keySize = KeySerializer.CalculateSize(keyContext, item.Key);
			SizeSerializer.Serialize(keySize, writer, context);
			KeySerializer.Serialize(item.Key, writer, context);
		}

		// write value
		using (var valueContext = SerializationContext.New) {
			var valueSize = ValueSerializer.CalculateSize(valueContext, item.Value);
			SizeSerializer.Serialize(valueSize, writer, context);
			ValueSerializer.Serialize(item.Value, writer, context);
		}
	}

	public override KeyValuePair<TKey, TValue> Deserialize(EndianBinaryReader reader, SerializationContext context) {
		// Deserialize key
		TKey key;
		TValue value;
		using (var keyContext = new SerializationContext()) {
			SizeSerializer.Deserialize(reader, context);
			key = KeySerializer.Deserialize(reader, context);
		}

		using (var valueContext = new SerializationContext()) {
			SizeSerializer.Deserialize(reader, context);
			value = ValueSerializer.Deserialize(reader, context);
		}

		return new KeyValuePair<TKey, TValue>(key, value);
	}

	public TKey DeserializeKey(EndianBinaryReader reader) {
		using var context = new SerializationContext();
		SizeSerializer.Deserialize(reader, context);
		return KeySerializer.Deserialize(reader, context);
	}


	public byte[] ReadKeyBytes(EndianBinaryReader reader) {
		using var context = new SerializationContext();
		var keyBytesLength = SizeSerializer.Deserialize(reader, context);
		return reader.ReadBytes(keyBytesLength);
	}

	public TValue DeserializeValue(EndianBinaryReader reader) {
		using var context = new SerializationContext();

		// skip key
		var keyBytesLength = SizeSerializer.Deserialize(reader, context);
		reader.BaseStream.Seek(keyBytesLength, SeekOrigin.Current);

		// read value 
		SizeSerializer.Deserialize(reader, context);
		return ValueSerializer.Deserialize(reader, context);
	}

	public byte[] ReadValueBytes(EndianBinaryReader reader) {
		using var context = new SerializationContext();

		// skip key
		var keyBytesLength = SizeSerializer.Deserialize(reader, context);
		reader.BaseStream.Seek(keyBytesLength, SeekOrigin.Current);

		// read value (size inferred from left-over bytes)
		var valueSize = SizeSerializer.Deserialize(reader, context);
		return reader.ReadBytes(valueSize);
	}
}
