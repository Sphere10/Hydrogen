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
/// </remarks>
public class KeyValuePairSerializer<TKey, TValue> : ItemSerializer<KeyValuePair<TKey, TValue>> {

	public KeyValuePairSerializer(IItemSerializer<TKey> keySerializer = null, IItemSerializer<TValue> valueSerializer = null, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt)
		: base(sizeDescriptorStrategy) {
		KeySerializer = keySerializer ?? ItemSerializer<TKey>.Default;
		ValueSerializer = (valueSerializer ?? ItemSerializer<TValue>.Default).AsSanitized();
	}

	public IItemSerializer<TKey> KeySerializer { get; }

	public IItemSerializer<TValue> ValueSerializer { get; }

	public override long CalculateSize(KeyValuePair<TKey, TValue> item) {
		var keySize = KeySerializer.CalculateSize(item.Key);
		var keySizeDescriptorSize = SizeSerializer.CalculateSize(keySize);
		var valueSize = ValueSerializer.CalculateSize(item.Value);
		var valueSizeDescriptorSize = SizeSerializer.CalculateSize(valueSize);
		return keySizeDescriptorSize + keySize + valueSizeDescriptorSize + valueSize;
	}

	public override void Serialize(KeyValuePair<TKey, TValue> item, EndianBinaryWriter writer) {
		// write key 
		var keySize = KeySerializer.CalculateSize(item.Key);
		SizeSerializer.Serialize(keySize, writer);
		KeySerializer.Serialize(item.Key, writer);

		// write value
		var valueSize = ValueSerializer.CalculateSize(item.Value);
		SizeSerializer.Serialize(valueSize, writer);
		ValueSerializer.Serialize(item.Value, writer);
	}

	public override KeyValuePair<TKey, TValue> Deserialize(EndianBinaryReader reader) {
		// Deserialize key
		SizeSerializer.Deserialize(reader);
		var key = KeySerializer.Deserialize(reader);
		SizeSerializer.Deserialize(reader);
		var value = ValueSerializer.Deserialize(reader);
		return new KeyValuePair<TKey, TValue>(key, value);
	}

	public TKey DeserializeKey(EndianBinaryReader reader) {
		SizeSerializer.Deserialize(reader);
		return KeySerializer.Deserialize(reader);
	}

	public byte[] ReadKeyBytes(EndianBinaryReader reader) {
		var keyBytesLength = SizeSerializer.Deserialize(reader);
		return reader.ReadBytes(keyBytesLength);
	}

	public TValue DeserializeValue(EndianBinaryReader reader) {
		// skip key
		var keyBytesLength = SizeSerializer.Deserialize(reader);
		reader.BaseStream.Seek(keyBytesLength, SeekOrigin.Current);

		// read value 
		SizeSerializer.Deserialize(reader);
		return ValueSerializer.Deserialize(reader);
	}

	public byte[] ReadValueBytes(EndianBinaryReader reader) {
		// skip key
		var keyBytesLength = SizeSerializer.Deserialize(reader);
		reader.BaseStream.Seek(keyBytesLength, SeekOrigin.Current);

		// read value (size inferred from left-over bytes)
		var valueSize = SizeSerializer.Deserialize(reader);
		return reader.ReadBytes(valueSize);
	}
}
