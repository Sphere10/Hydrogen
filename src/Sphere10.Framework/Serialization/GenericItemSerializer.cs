using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using Sphere10.Framework.FastReflection;

namespace Sphere10.Framework {
	public class GenericItemSerializer<T> : IItemSerializer<T> where T : new() {
		
		private static readonly Dictionary<Type, ushort> _typeRegistry = new ();

		private readonly EndianBitConverter _bitConverter = EndianBitConverter.Little;

		private Encoding _stringEncoding;
		
		private int _headerByteSize;

		public bool IsFixedSize { get; }
		
		public int FixedSize { get; }
		
		public static CHF TypeNameHashAlgorithm { get; set; } = CHF.Blake2b_128;

		public static IItemSerializer<T> Default { get; } = new GenericItemSerializer<T>();

		static GenericItemSerializer() {
			
		}

		public GenericItemSerializer() {
			_headerByteSize = Hashers.GetDigestSizeBytes(TypeNameHashAlgorithm);
			
			Register<string>();
			Register<char>();
			Register<int>();
			Register<uint>();
			Register<double>();
			Register<ushort>();
		}

		public int CalculateTotalSize(IEnumerable<T> items, bool calculateIndividualItems, out int[] itemSizes) {
			throw new NotImplementedException();
		}

		public int CalculateSize(T item) {
			throw new NotImplementedException();
		}

		public int Serialize(T @object, EndianBinaryWriter writer) {
			_stringEncoding = writer.Encoding;
			
			var properties = @object.GetType().GetProperties();
			int byteCount = 0;
			foreach (PropertyInfo propertyInfo in properties) {
				var value = propertyInfo.FastGetValue(@object);
				var bytes = SerializeProperty(propertyInfo.PropertyType, value);
				writer.Write(bytes);
				byteCount += bytes.Length;
			}

			return byteCount;
		}

		public T Deserialize(int size, EndianBinaryReader reader) {
			var properties = typeof(T).GetProperties();

			T instance = new T();
			foreach (var propertyInfo in properties) {
				propertyInfo.FastSetValue(instance, DeserializeProperty(propertyInfo.PropertyType, reader));
			}

			return instance;
		}
		
		public static void Register<TType>() {
			var type = typeof(TType);
			
			if (_typeRegistry.ContainsKey(type))
				throw new InvalidOperationException($"Type {typeof(TType)} is already registered");

			ushort last = 0;
			if (_typeRegistry.Any())
				last = _typeRegistry.LastOrDefault().Value;

			last++;
			_typeRegistry.Add(type, last);
		}

		private object DeserializeProperty(Type propertyType, EndianBinaryReader reader) {
			_stringEncoding = reader.Encoding;
			
			if (propertyType.IsPrimitive)
				return DeserializePrimitive(propertyType, reader);

			if (propertyType == typeof(decimal))
				return reader.ReadDecimal();

			if (propertyType == typeof(string))
				return DeserializeString(reader);

			if (propertyType.IsCollection())
				return DeserializeCollection(reader);

			return DeserializeComplexType(propertyType, reader);
		}

		private object DeserializePrimitive(Type t, EndianBinaryReader reader) {

			if (t == typeof(sbyte))
				return reader.ReadSByte();

			if (t == typeof(byte))
				return reader.ReadByte();
			
			if (t == typeof(short))
				return reader.ReadInt16();
			
			if (t == typeof(ushort))
				return reader.ReadUInt16();
			
			if (t == typeof(int))
				return reader.ReadInt32();
			
			if (t == typeof(uint))
				return reader.ReadUInt32();
			
			if (t == typeof(long))
				return reader.ReadInt64();
			
			if (t == typeof(ulong))
				return reader.ReadUInt64();
			
			if (t == typeof(double))
				return reader.ReadDouble();
			
			if (t == typeof(bool))
				return reader.ReadBoolean();

			if (t == typeof(char))
				return _bitConverter.ToChar(reader.ReadBytes(2));

			if (t == typeof(float))
				return reader.ReadSingle();

			throw new InvalidOperationException($"Exception while deserializing, unknown primitive type {t.Name}");
		}

		private string DeserializeString(EndianBinaryReader reader) => reader.ReadString();
		
		private object DeserializeCollection(EndianBinaryReader reader) {
			var collectionType = ReadTypeHeader(reader);
			var count = reader.ReadInt32();

			List<object> objects = new List<object>();
			for (int i = 0; i < count; i++) {
				var itemType = ReadTypeHeader(reader);
				objects.Add(DeserializeProperty(itemType, reader));
			}
			
			// Returning array, need to convert to 'collectionType'
			return Activator.CreateInstance(collectionType, objects.ToArray());
		}

		private object DeserializeComplexType(Type propertyType, EndianBinaryReader reader) {
			Type valueType = propertyType;
			if (!propertyType.IsValueType || !propertyType.IsSealed) 
				valueType = ReadTypeHeader(reader);
			
			if (propertyType.IsAssignableFrom(valueType)) {
				var serializer = GenericItemSerializer<object>.Default;
				return serializer.Deserialize(0, reader);
			} else
				throw new InvalidOperationException($"Found type {valueType.Name} to deserialize, but target property {propertyType.Name} is not assignable from {propertyType.Name}");
		}

		private byte[] SerializeProperty(Type propertyType, object propertyValue) {

			if (propertyType.IsPrimitive)
				return SerializePrimitive(propertyValue);

			if (propertyType == typeof(decimal))
				return _bitConverter.GetBytes((decimal)propertyValue);

			if (propertyType == typeof(string))
				return SerializeString(propertyValue as string);

			if (propertyType.IsCollection())
				return SerializeCollectionType(propertyValue);
			
			return SerializeReferenceType(propertyType, propertyValue);
		}

		private byte[] SerializePrimitive(object boxedPrimitive) {
			return boxedPrimitive switch {
				sbyte x => Array.ConvertAll(new []{x}, input => (byte)input),
				byte x => new []{x},
				short x => _bitConverter.GetBytes(x),
				ushort x => _bitConverter.GetBytes(x),
				int x => _bitConverter.GetBytes(x),
				uint x => _bitConverter.GetBytes(x),
				long x => _bitConverter.GetBytes(x),
				ulong x => _bitConverter.GetBytes(x),
				float x => _bitConverter.GetBytes(x),
				double x => _bitConverter.GetBytes(x),
				decimal x => _bitConverter.GetBytes(x),
				bool x => _bitConverter.GetBytes(x),
				char x => _bitConverter.GetBytes(x),
				_ => throw new InvalidOperationException($"Unknown primitive type")
			};
		}
		
		private byte[] SerializeReferenceType(Type propertyType, object propertyValue) {
			using MemoryStream stream = new MemoryStream();
			using EndianBinaryWriter writer = new EndianBinaryWriter(_bitConverter, stream);
				
			if (!propertyType.IsValueType || !propertyType.IsSealed)
				writer.Write(GetTypeIndex(propertyValue.GetType()));
			
			//object serializer, replace with specific type serializer, which has caching of the type's propertyinfo for faster
			//serialization. Will need construction from type, non generic GenericSerializer ? 
			var serializer = GenericItemSerializer<object>.Default;
			serializer.Serialize(propertyValue, writer);

			return stream.ToArray();
		}

		private byte[] SerializeString(string value) {
			using var stream = new MemoryStream();
			using var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream);
			writer.Write(value);
			return stream.ToArray();
		}

		private byte[] SerializeCollectionType(object propertyValue) {
			using MemoryStream stream = new MemoryStream();
			using EndianBinaryWriter writer = new EndianBinaryWriter(_bitConverter, stream);
			
			if (propertyValue is IEnumerable enumerable) {
				var index = GetTypeIndex(propertyValue.GetType());
				var enumerator = enumerable.GetEnumerator();
				var count = 0;

				// Replace with item specific serializer
				var itemSerializer = GenericItemSerializer<object>.Default;

				while (enumerator.MoveNext()) {
					var itemType = enumerator.Current!.GetType();
					writer.Write(GetTypeIndex(itemType));
					writer.Write(SerializeProperty(itemType, enumerator.Current));
					count++;
				}

				return Tools.Array.Concat<byte>(_bitConverter.GetBytes(index),_bitConverter.GetBytes(count), stream.ToArray());
			}
			else
				throw new InvalidOperationException("Could not serialize object as it doesn't implement IEnumerable.");
		}

		private ushort GetTypeIndex(Type type) {
			return _typeRegistry.ContainsKey(type) ? _typeRegistry[type] : throw new InvalidOperationException($"Type {type.Name} is not registered, add using Register<T>()");
		}

		private Type ReadTypeHeader(EndianBinaryReader reader) {
			var typeIndex = reader.ReadUInt16();

			if (_typeRegistry.ContainsValue(typeIndex)) {
				// Performance problem, iterating dictionary to match value.
				return _typeRegistry.FirstOrDefault(x => x.Value == typeIndex).Key;
			} else
				throw new InvalidOperationException($"Found unknown type index {typeIndex} during deserialization of value for property");
		}
	}
}
