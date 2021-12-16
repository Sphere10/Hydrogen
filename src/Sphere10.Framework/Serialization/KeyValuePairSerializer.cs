using System;
using System.Collections.Generic;
using System.Linq;
using Sphere10.Framework.Values;

namespace Sphere10.Framework {


	/// <summary>
	/// Serializes a <see cref="KeyValuePair{TKey, TValue}"/> using component serializers for <see cref="TKey"/> and <see cref="TValue"/>. 
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <remarks>Serialization format: [KeyLength (uint32)] [KeyBytes....] [ValueLength (uint32)] [ValueBytes....] where</remarks>
	public class KeyValuePairSerializer<TKey, TValue> : ItemSerializerBase<KeyValuePair<TKey, TValue>> {
		private readonly IItemSerializer<TKey> _keySerializer;
		private readonly IItemSerializer<TValue> _valueSerializer;

		public KeyValuePairSerializer(IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer) {
			_keySerializer = keySerializer;
			_valueSerializer = valueSerializer;
		}

		public override int CalculateSize(KeyValuePair<TKey, TValue> item) {
			var size = 0;
			size +=  _keySerializer.CalculateSize(item.Key);
			size += _valueSerializer.CalculateSize(item.Value);
			if (item.Value != null) {
				size += sizeof(int);
				size += sizeof(int);
			}
			return size;
		}

		public override bool TrySerialize(KeyValuePair<TKey, TValue> item, EndianBinaryWriter writer, out int bytesWritten) {
			bytesWritten = 0;

			if (!_keySerializer.TrySerialize(item.Key, out var keyBytes, writer.BitConverter.Endianness))
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
			if (sizeof(int) + keyBytesLength < byteSize) {
				var valueBytesLength = reader.ReadUInt32();
				if (!_valueSerializer.TryDeserialize(reader.ReadBytes((int)valueBytesLength), out value, reader.BitConverter.Endianness))
					return false;
			}
			item = new KeyValuePair<TKey, TValue>(key, value);
			return true;
		}
	}
}
