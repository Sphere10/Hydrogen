using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sphere10.Framework.Values;

namespace Sphere10.Framework {


	/// <summary>
	/// Serializes a <see cref="KeyValuePair{TKey, TValue}"/> using component serializers for <see cref="TKey"/> and <see cref="TValue"/>. 
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <remarks>Serialization format: [KeyLength (uint32)] [KeyBytes....] [ValueLength (uint32)] [ValueBytes....] such that if total KVP bytes excludes ValueLength, than Value is inferred null</remarks>
	public class KeyValuePairSerializer<TKey, TValue> : ItemSerializer<KeyValuePair<TKey, TValue>> {
		private readonly IItemSerializer<TKey> _keySerializer;
		private readonly IItemSerializer<TValue> _valueSerializer;

		public KeyValuePairSerializer(IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer) {
			_keySerializer = keySerializer;
			_valueSerializer = valueSerializer;
		}

		public override int CalculateSize(KeyValuePair<TKey, TValue> item) {
			var size = sizeof(uint);
			if (item.Key != null)
				size += _keySerializer.CalculateSize(item.Key);
			if (item.Value != null) {
				size += sizeof(int);
				size += _valueSerializer.CalculateSize(item.Value);
			}
			return size;
		}

		public override bool TrySerialize(KeyValuePair<TKey, TValue> item, EndianBinaryWriter writer, out int bytesWritten) {
			bytesWritten = 0;

			var keyBytes = Array.Empty<byte>();
			if (item.Key != null)
				if (!_keySerializer.TrySerialize(item.Key, out keyBytes, writer.BitConverter.Endianness))
					return false;

			writer.Write(keyBytes.Length);
			writer.Write(keyBytes);
			bytesWritten = sizeof(int) + keyBytes.Length;
			if (item.Value != null) {
				if (!_valueSerializer.TrySerialize(item.Value, out var valueBytes, writer.BitConverter.Endianness))
					return false;

				writer.Write(valueBytes.Length);
				writer.Write(valueBytes);
				bytesWritten += sizeof(int) + valueBytes.Length;
			}
			
			return true;
		}

		public override bool TryDeserialize(int byteSize, EndianBinaryReader reader, out KeyValuePair<TKey, TValue> item) {
			// TODO: add max length checks on reading byte lengths to avoid attacks with malicious kvp's
			item = default;
			var keyBytesLength = reader.ReadUInt32();
			if (!_keySerializer.TryDeserialize(reader.ReadBytes((int)keyBytesLength), out var key, reader.BitConverter.Endianness))
				return false;
			TValue value = default;
			if (sizeof(uint) + keyBytesLength < byteSize) {
				var valueBytesLength = reader.ReadUInt32();
				if (!_valueSerializer.TryDeserialize(reader.ReadBytes((int)valueBytesLength), out value, reader.BitConverter.Endianness))
					return false;
			}
			item = new KeyValuePair<TKey, TValue>(key, value);
			return true;
		}

		public bool TryDeserializeKey(int byteSize, EndianBinaryReader reader, out TKey item) {
			// TODO: add max length checks on reading byte lengths to avoid attacks with malicious kvp's
			var keyBytesLength = reader.ReadUInt32();
			if (!_keySerializer.TryDeserialize(reader.ReadBytes((int)keyBytesLength), out item, reader.BitConverter.Endianness))
				return false;
			return true;
		}

		public bool TryDeserializeValue(int byteSize, EndianBinaryReader reader, out TValue item) {
			// TODO: add max length checks on reading byte lengths to avoid attacks with malicious kvp's
			item = default;
			var keyBytesLength = reader.ReadUInt32();
			reader.BaseStream.Seek(keyBytesLength, SeekOrigin.Current);
			if (sizeof(uint) + keyBytesLength < byteSize) {
				var valueBytesLength = reader.ReadUInt32();
				if (!_valueSerializer.TryDeserialize(reader.ReadBytes((int)valueBytesLength), out item, reader.BitConverter.Endianness))
					return false;
			}
			return true;
		}

		public TKey DeserializeKey(int byteSize, EndianBinaryReader reader) {
			if (!TryDeserializeKey(byteSize, reader, out var item))
				throw new InvalidOperationException($"Unable to deserialize key from KVP of size {byteSize}b");
			return item;
		}

		public TValue DeserializeValue(int byteSize, EndianBinaryReader reader) {
			if (!TryDeserializeValue(byteSize, reader, out var item))
				throw new InvalidOperationException($"Unable to deserialize value from KVP of size {byteSize}b");
			return item;
		}

	}
}
