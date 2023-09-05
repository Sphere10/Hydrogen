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
///  - A key-size is always given. Support for NULL keys can be achieved through the null-supporting key serializer (<see cref="NullableObjectSerializer{T}"/>)
///  - A value-size is only given if the value is not null. Null values are supported by not writing a value-size descriptor.
/// </remarks>
// TODO: add max length checks on reading byte lengths to avoid attacks with malicious kvp's
public class KeyValuePairSerializer<TKey, TValue> : ItemSerializer<KeyValuePair<TKey, TValue>> {

	private readonly IItemSerializer<TKey> _keySerializer;
	private readonly IItemSerializer<TValue> _valueSerializer;
	private readonly SizeDescriptorSerializer _sizeDescriptorSerializer;

	public KeyValuePairSerializer(IItemSerializer<TKey> keySerializer = null, IItemSerializer<TValue> valueSerializer = null, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) {
		_keySerializer = keySerializer ?? ItemSerializer<TKey>.Default;
		_valueSerializer = valueSerializer ?? ItemSerializer<TValue>.Default;
		_sizeDescriptorSerializer = new SizeDescriptorSerializer(sizeDescriptorStrategy);
	}

	public IItemSerializer<TKey> KeySerializer => _keySerializer;

	public IItemSerializer<TValue> ValueSerializer => _valueSerializer;

	public override long CalculateSize(KeyValuePair<TKey, TValue> item) {
		var keySize = _keySerializer.CalculateSize(item.Key);
		var keySizeDescriptorSize = _sizeDescriptorSerializer.CalculateSize(keySize);
		var valueSize = item.Value is not null ? _valueSerializer.CalculateSize(item.Value) : 0;
		var valueDescriptorSize = _sizeDescriptorSerializer.CalculateSize(valueSize);
		return keySizeDescriptorSize + keySize + (item.Value is not null ? valueDescriptorSize + valueSize : 0);
	}

	public override void SerializeInternal(KeyValuePair<TKey, TValue> item, EndianBinaryWriter writer) {
		// NOTE: key is not null-checked, as it is job of key serializer to support null keys

		// write key size
		var keySize = _keySerializer.CalculateSize(item.Key);
		_sizeDescriptorSerializer.SerializeInternal(keySize, writer);

		// write key if applicable
		var keyBytesWritten = _keySerializer.Serialize(item.Key, writer);
		Guard.Ensure(keyBytesWritten == keySize, "Key serialization overflow");

		// write value if applicable
		if (item.Value is not null) { 
			var valueSize = _valueSerializer.CalculateSize(item.Value);
			_sizeDescriptorSerializer.SerializeInternal(valueSize, writer);
			_valueSerializer.Serialize(item.Value, writer);
		}
	}

	public override KeyValuePair<TKey, TValue> DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		// NOTE: key is not null-checked and it is job of key serializer to support null keys if required

		// Deserialize key
		var keyBytesLength = _sizeDescriptorSerializer.Deserialize(reader);
		var key = _keySerializer.Deserialize(keyBytesLength, reader);

		// Deserialize value (let over bytes are inferred to be value bytes)
		var keySizeDescriptorSize = _sizeDescriptorSerializer.CalculateSize(keyBytesLength);

		if (byteSize - keySizeDescriptorSize - keyBytesLength == 0)
			return new KeyValuePair<TKey, TValue>(key, default); 	// null value
		
		var valueSize = _sizeDescriptorSerializer.Deserialize(reader);
		var value = _valueSerializer.Deserialize(valueSize, reader);
		return new KeyValuePair<TKey, TValue>(key, value);
	}

	public TKey DeserializeKey(EndianBinaryReader reader) {
		// NOTE: key is not null-checked and it is job of key serializer to support null keys if required
		var keyBytesLength = _sizeDescriptorSerializer.Deserialize(reader);
		return _keySerializer.Deserialize(keyBytesLength, reader);
	}

	public byte[] ReadKeyBytes(EndianBinaryReader reader) {
		// NOTE: key is not null-checked and it is job of key serializer to support null keys if required
		var keyBytesLength = _sizeDescriptorSerializer.Deserialize(reader);
		return reader.ReadBytes(keyBytesLength);
	}

	public TValue DeserializeValue(long byteSize, EndianBinaryReader reader) {
		// NOTE: key is not null-checked and it is job of key serializer to support null keys if required

		// skip key
		var keyBytesLength = _sizeDescriptorSerializer.Deserialize(reader);
		reader.BaseStream.Seek(keyBytesLength, SeekOrigin.Current);
		var keySizeDescriptorSize = _sizeDescriptorSerializer.CalculateSize(keyBytesLength);

		if (byteSize - keySizeDescriptorSize - keyBytesLength == 0)
			return default; 	// null value

		// read value (size inferred from left-over bytes)
		var valueSize = _sizeDescriptorSerializer.Deserialize(reader);
		return _valueSerializer.Deserialize(valueSize, reader);
	}

	public byte[] ReadValueBytes(long totalKvpBytes, EndianBinaryReader reader) {
		// NOTE: key is not null-checked and it is job of key serializer to support null keys if required

		// skip key
		var keyBytesLength = _sizeDescriptorSerializer.Deserialize(reader);
		reader.BaseStream.Seek(keyBytesLength, SeekOrigin.Current);
		var keySizeDescriptorSize = _sizeDescriptorSerializer.CalculateSize(keyBytesLength);

		if (totalKvpBytes - keySizeDescriptorSize - keyBytesLength == 0)
			return null; 	// null value

		// read value (size inferred from left-over bytes)
		var valueSize = _sizeDescriptorSerializer.Deserialize(reader);
		return reader.ReadBytes(valueSize);
	}
}
