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
/// <remarks>Serialization format: [KeyLength (uint32)] [KeyBytes....] [ValueLength (uint32)] [ValueBytes....] such that if total KVP bytes excludes ValueLength, than Value is inferred null</remarks>
public class KeyValuePairSerializer<TKey, TValue> : ItemSerializer<KeyValuePair<TKey, TValue>> {

	private readonly IItemSerializer<TKey> _keySerializer;
	private readonly IItemSerializer<TValue> _valueSerializer;
	private readonly SizeDescriptorSerializer _sizeDescriptorSerializer;

	public KeyValuePairSerializer(IItemSerializer<TKey> keySerializer = null, IItemSerializer<TValue> valueSerializer = null, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseUInt32) {
		_keySerializer = keySerializer ?? ItemSerializer<TKey>.Default;
		_valueSerializer = valueSerializer ?? ItemSerializer<TValue>.Default;
		_sizeDescriptorSerializer = new SizeDescriptorSerializer(sizeDescriptorStrategy);
	}
	public IItemSerializer<TKey> KeySerializer => _keySerializer;

	public IItemSerializer<TValue> ValueSerializer => _valueSerializer;

	public override long CalculateSize(KeyValuePair<TKey, TValue> item) {
		var keySize = item.Key is not null ? _keySerializer.CalculateSize(item.Key) : 0;
		var keySizeDescriptorSize = _sizeDescriptorSerializer.CalculateSize(keySize);
		var valueSize = item.Value is not null ? _valueSerializer.CalculateSize(item.Value) : 0;
		var valueSizeDescriptorSize = _sizeDescriptorSerializer.CalculateSize(valueSize);
		return keySizeDescriptorSize + keySize + (item.Value is not null ? valueSizeDescriptorSize + valueSize : 0);
	}

	public override bool TrySerialize(KeyValuePair<TKey, TValue> item, EndianBinaryWriter writer, out long bytesWritten) {
		bytesWritten = 0;

		// write key size
		var keySize = item.Key is not null ? _keySerializer.CalculateSize(item.Key) : 0;
		if (!_sizeDescriptorSerializer.TrySerialize(keySize, writer, out var keySizeDescriptorBytesWritten))
			return false;
		bytesWritten += keySizeDescriptorBytesWritten;

		// write key if applicable
		if (item.Key is not null) {
			if (!_keySerializer.TrySerialize(item.Key, writer, out var keyBytesWritten))
				return false;
			Debug.Assert(keyBytesWritten == keySize);
			bytesWritten += keyBytesWritten;
		}

		if (item.Value is not null) {
			var valueSize = _valueSerializer.CalculateSize(item.Value);
			if (!_sizeDescriptorSerializer.TrySerialize(valueSize, writer, out var valueSizeDescriptorBytesWritten))
				return false;
			bytesWritten += valueSizeDescriptorBytesWritten;

			if (!_valueSerializer.TrySerialize(item.Value, writer, out var valueBytesWritten))
				return false;
			Debug.Assert(valueBytesWritten == valueSize);
			bytesWritten += valueBytesWritten;
		}

		return true;
	}

	public override bool TryDeserialize(long byteSize, EndianBinaryReader reader, out KeyValuePair<TKey, TValue> item) {
		// TODO: add max length checks on reading byte lengths to avoid attacks with malicious kvp's
		item = default;

		// Deserialize key
		if (!_sizeDescriptorSerializer.TryDeserialize(reader, out var keyBytesLength))
			return false;

		var keyBytesLengthI = Tools.Collection.CheckNotImplemented64bitAddressingLength(keyBytesLength);
		if (!_keySerializer.TryDeserialize(reader.ReadBytes(keyBytesLengthI), out var key, reader.BitConverter.Endianness))
			return false;

		// Deserialize value
		TValue value = default;
		var keySizeDescriptorSize = _sizeDescriptorSerializer.CalculateSize(keyBytesLength);
		if (keySizeDescriptorSize + keyBytesLength < byteSize) {
			// there are left-over bytes which must be the non-null value

			if (!_sizeDescriptorSerializer.TryDeserialize(reader, out var valueBytesLength))
				return false;

			var valueBytesLengthI = Tools.Collection.CheckNotImplemented64bitAddressingLength(valueBytesLength);
			if (!_valueSerializer.TryDeserialize(reader.ReadBytes(valueBytesLengthI), out value, reader.BitConverter.Endianness))
				return false;
		}
		item = new KeyValuePair<TKey, TValue>(key, value);
		return true;
	}

	public bool TryDeserializeKey(long byteSize, EndianBinaryReader reader, out TKey item) {
		// TODO: add max length checks on reading byte lengths to avoid attacks with malicious kvp's
		item = default;
		if (!_sizeDescriptorSerializer.TryDeserialize(reader, out var keyBytesLength))
			return false;

		var keyBytesLengthI = Tools.Collection.CheckNotImplemented64bitAddressingLength(keyBytesLength);
		if (!_keySerializer.TryDeserialize(reader.ReadBytes(keyBytesLengthI), out item, reader.BitConverter.Endianness))
			return false;

		return true;
	}

	public bool TryDeserializeValue(long byteSize, EndianBinaryReader reader, out TValue item) {
		// TODO: add max length checks on reading byte lengths to avoid attacks with malicious kvp's
		item = default;

		// skip key
		if (!_sizeDescriptorSerializer.TryDeserialize(reader, out var keyBytesLength))
			return false;
		reader.BaseStream.Seek(keyBytesLength, SeekOrigin.Current);

		// read value
		var keySizeDescriptorSize = _sizeDescriptorSerializer.CalculateSize(keyBytesLength);
		if (keySizeDescriptorSize + keyBytesLength < byteSize) {
			if (!_sizeDescriptorSerializer.TryDeserialize(reader, out var valueBytesLength))
				return false;
			var valueBytesLengthI = Tools.Collection.CheckNotImplemented64bitAddressingLength(valueBytesLength);
			if (!_valueSerializer.TryDeserialize(reader.ReadBytes(valueBytesLengthI), out item, reader.BitConverter.Endianness))
				return false;
		}
		return true;
	}

	public TKey DeserializeKey(long byteSize, EndianBinaryReader reader) {
		if (!TryDeserializeKey(byteSize, reader, out var item))
			throw new InvalidOperationException($"Unable to deserialize key from KVP of size {byteSize}b");
		return item;
	}

	public TValue DeserializeValue(long byteSize, EndianBinaryReader reader) {
		if (!TryDeserializeValue(byteSize, reader, out var item))
			throw new InvalidOperationException($"Unable to deserialize value from KVP of size {byteSize}b");
		return item;
	}

}
