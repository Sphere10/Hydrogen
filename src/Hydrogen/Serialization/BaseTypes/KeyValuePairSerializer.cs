// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
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
public class KeyValuePairSerializer<TKey, TValue> : ItemSerializerBase<KeyValuePair<TKey, TValue>>, IKeyValuePairSerializer<TKey, TValue> {
	private readonly SizeDescriptorSerializer _sizeSerializer;
	public KeyValuePairSerializer(IItemSerializer<TKey> keySerializer = null, IItemSerializer<TValue> valueSerializer = null, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) {
		KeySerializer = keySerializer ?? ItemSerializer<TKey>.Default;
		ValueSerializer = (valueSerializer ?? ItemSerializer<TValue>.Default);
		_sizeSerializer = new SizeDescriptorSerializer(sizeDescriptorStrategy);
	}

	public IItemSerializer<TKey> KeySerializer { get; }

	public IItemSerializer<TValue> ValueSerializer { get; }

	public override long CalculateSize(SerializationContext context, KeyValuePair<TKey, TValue> item) {
		var keySize = KeySerializer.CalculateSize(context, item.Key);
		var keySizeDescriptorSize = _sizeSerializer.CalculateSize(context, keySize);
		var valueSize = ValueSerializer.CalculateSize(context, item.Value);
		var valueSizeDescriptorSize = _sizeSerializer.CalculateSize(context, valueSize);
		return keySizeDescriptorSize + keySize + valueSizeDescriptorSize + valueSize;
	}

	public override void Serialize(KeyValuePair<TKey, TValue> item, EndianBinaryWriter writer, SerializationContext context) {
		// write key
		var keySize = KeySerializer.CalculateSize(context, item.Key);
		_sizeSerializer.Serialize(keySize, writer, context);
		KeySerializer.Serialize(item.Key, writer, context);
		
		// write value
		var valueSize = ValueSerializer.CalculateSize(context, item.Value);
		_sizeSerializer.Serialize(valueSize, writer, context);
		ValueSerializer.Serialize(item.Value, writer, context);
	}

	public override KeyValuePair<TKey, TValue> Deserialize(EndianBinaryReader reader, SerializationContext context) {
		// Deserialize key

		_sizeSerializer.Deserialize(reader, context);
		var key = KeySerializer.Deserialize(reader, context);
		
		_sizeSerializer.Deserialize(reader, context);
		var value = ValueSerializer.Deserialize(reader, context);

		return new KeyValuePair<TKey, TValue>(key, value);
	}

	public TKey DeserializeKey(EndianBinaryReader reader)  // NOTE: potential issue since using a new context here
		=> Deserialize(reader, SerializationContext.New).Key;

	public byte[] ReadKeyBytes(EndianBinaryReader reader) {
		using var context = new SerializationContext();
		var keyBytesLength = _sizeSerializer.Deserialize(reader, context);
		return reader.ReadBytes(keyBytesLength);
	}

	public TValue DeserializeValue(EndianBinaryReader reader) // NOTE: potential issue since using a new context here
		=> Deserialize(reader, SerializationContext.New).Value;

	public byte[] ReadValueBytes(EndianBinaryReader reader) {
		var context = new SerializationContext();

		// skip key
		var keyBytesLength = _sizeSerializer.Deserialize(reader, context);
		reader.BaseStream.Seek(keyBytesLength, SeekOrigin.Current);

		// read value (size inferred from left-over bytes)
		var valueSize = _sizeSerializer.Deserialize(reader, context);
		return reader.ReadBytes(valueSize);
	}

}
