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
/// Serializes a <see cref="KeyValuePair{TKey, TValue}"/> using component serializers for <see cref="TKey"/> and <see cref="TValue"/>. However, the key and value are
/// serialized in separate contexts which allows them to be deserialized independently. This is not the case for <see cref="KeyValuePair{TKey, TValue}"/> which
/// uses a single context for both key and value serializations and which may reference each other (i.e. an object in value could be a reference to an object in key).
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <remarks>Serialization format: [KeySize] [KeyBytes....] [ValueSize] [ValueBytes....]
///  - A key-size is always serialized so it can be skipped when only reading values.
///  - Null values are supported (even if value serializer doesn't)
///  - Null keys are not supported by default, but passing a key serializer that supports null will
///  - Since keys/values are accessible independently, they use their own context thus cannot share references from previous context. This needs to be addressed later.
/// </remarks>
public class ContextSegregatedKeyValuePairSerializer<TKey, TValue> : ItemSerializerBase<KeyValuePair<TKey, TValue>>, IKeyValuePairSerializer<TKey, TValue> {
	private readonly SizeDescriptorSerializer _sizeSerializer;
	public ContextSegregatedKeyValuePairSerializer(IItemSerializer<TKey> keySerializer = null, IItemSerializer<TValue> valueSerializer = null, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) {
		KeySerializer = keySerializer ?? ItemSerializer<TKey>.Default;
		ValueSerializer = valueSerializer ?? ItemSerializer<TValue>.Default;
		_sizeSerializer = new SizeDescriptorSerializer(sizeDescriptorStrategy);
	}

	public IItemSerializer<TKey> KeySerializer { get; }

	public IItemSerializer<TValue> ValueSerializer { get; }

	public override long CalculateSize(SerializationContext context, KeyValuePair<TKey, TValue> item) {
		long keySize, keySizeDescriptorSize, valueSize, valueSizeDescriptorSize;
		using (var keyContext = SerializationContext.New) {
			keySize = KeySerializer.CalculateSize(keyContext, item.Key);
			keySizeDescriptorSize = _sizeSerializer.CalculateSize(keyContext, keySize);
		}
		
		using (var valueContext = SerializationContext.New) {
			valueSize = ValueSerializer.CalculateSize(valueContext, item.Value);
			valueSizeDescriptorSize = _sizeSerializer.CalculateSize(valueContext, valueSize);
		}
		return keySizeDescriptorSize + keySize + valueSizeDescriptorSize + valueSize;
	}

	public override void Serialize(KeyValuePair<TKey, TValue> item, EndianBinaryWriter writer, SerializationContext context) {
		// NOTE: since keys/values are accessible independently, we must use a fresh context for each

		// key value		
		using (var keyContext = SerializationContext.New) {
			var keySize = KeySerializer.CalculateSize(keyContext, item.Key);
			_sizeSerializer.Serialize(keySize, writer, keyContext);
			KeySerializer.Serialize(item.Key, writer, keyContext);
		}

		// write value
		using (var valueContext = SerializationContext.New) {
			var valueSize = ValueSerializer.CalculateSize(valueContext, item.Value);
			_sizeSerializer.Serialize(valueSize, writer, valueContext);
			ValueSerializer.Serialize(item.Value, writer, valueContext);
		}
	}

	public override KeyValuePair<TKey, TValue> Deserialize(EndianBinaryReader reader, SerializationContext context) {
		// Deserialize key
		TKey key;
		TValue value;
		using (var keyContext = new SerializationContext()) {
			_sizeSerializer.Deserialize(reader, keyContext);
			key = KeySerializer.Deserialize(reader, keyContext);
		}

		using (var valueContext = new SerializationContext()) {
			_sizeSerializer.Deserialize(reader, valueContext);
			value = ValueSerializer.Deserialize(reader, valueContext);
		}

		return new KeyValuePair<TKey, TValue>(key, value);
	}

	public TKey DeserializeKey(EndianBinaryReader reader) {
		using var context = new SerializationContext();
		_sizeSerializer.Deserialize(reader, context);
		return KeySerializer.Deserialize(reader, context);
	}


	public byte[] ReadKeyBytes(EndianBinaryReader reader) {
		using var context = new SerializationContext();
		var keyBytesLength = _sizeSerializer.Deserialize(reader, context);
		return reader.ReadBytes(keyBytesLength);
	}

	public TValue DeserializeValue(EndianBinaryReader reader) {
		using var context = new SerializationContext();

		// skip key
		var keyBytesLength = _sizeSerializer.Deserialize(reader, context);
		reader.BaseStream.Seek(keyBytesLength, SeekOrigin.Current);

		// read value 
		_sizeSerializer.Deserialize(reader, context);
		return ValueSerializer.Deserialize(reader, context);
	}

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
