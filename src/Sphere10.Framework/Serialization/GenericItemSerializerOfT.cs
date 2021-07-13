using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sphere10.Framework.FastReflection;

namespace Sphere10.Framework {
	public class GenericItemSerializer<T> : GenericItemSerializer, IItemSerializer<T> where T : new() {
		// refactor - shared fields set during serialization/deserialization aren't threadsafe. Using 'Default' static instance
		// makes this a singleton, so needs to be threadsafe. 
		private EndianBitConverter _bitConverter;

		public bool IsFixedSize { get; }

		public int FixedSize { get; }

		//implement static singleton pattern better with Lazy<T>
		public static IItemSerializer<T> Default { get; } = new GenericItemSerializer<T>();

		public int CalculateTotalSize(IEnumerable<T> items, bool calculateIndividualItems, out int[] itemSizes) {
			throw new NotImplementedException();
		}

		public int CalculateSize(T item) {
			throw new NotImplementedException();
		}

		public int Serialize(T @object, EndianBinaryWriter writer) {
			_bitConverter = writer.BitConverter;

			var byteCount = 0;

			foreach (var field in GetSerializableProperties(@object.GetType())) {
				var value = field.FastGetValue(@object);
				var bytes = SerializeMember(value);
				writer.Write(bytes);
				byteCount += bytes.Length;
			}

			return byteCount;
		}

		public T Deserialize(int size, EndianBinaryReader reader) {
			_bitConverter = reader.BitConverter;
			var instance = new T();

			var properties = GetSerializableProperties(typeof(T));
			foreach (var propertyInfo in properties) {
				var value = DeserializeMember(propertyInfo.PropertyType, reader);
				propertyInfo.FastSetValue(instance, value);
			}

			return instance;
		}

		private object DeserializeMember(Type memberType, EndianBinaryReader reader) {
			if (memberType.IsPrimitive)
				return DeserializePrimitive(memberType, reader);

			if (memberType == typeof(string))
				return DeserializeString(reader);

			if (memberType.IsValueType)
				return DeserializeValueType(memberType, reader);

			if (memberType.IsCollection())
				return DeserializeCollection(reader);

			return DeserializeReferenceType(memberType, reader);

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
				return reader.BitConverter.ToChar(reader.ReadBytes(2));

			if (t == typeof(float))
				return reader.ReadSingle();

			throw new InvalidOperationException($"Exception while deserializing, unknown primitive type {t.Name}");
		}

		private string DeserializeString(EndianBinaryReader reader) {
			var type = ReadTypeHeader(reader);

			if (type == typeof(NullValue))
				return null;

			if (type == typeof(string))
				return reader.ReadString();
			throw new InvalidOperationException($"Expected string type header, found {type.Name}");
		}

		private object DeserializeCollection(EndianBinaryReader reader) {
			var collectionType = ReadTypeHeader(reader);
			object list;

			if (collectionType == typeof(Array)) {
				var elementType = ReadTypeHeader(reader);
				var count = reader.ReadInt32();

				var elements = new object[count];
				for (var i = 0; i < count; i++) {
					var obj = DeserializeMember(elementType, reader);
					elements[i] = obj;
				}

				var array = Array.CreateInstance(elementType, elements.Length);
				elements.CopyTo(array, 0);
				list = array;
			} else if (typeof(IDictionary).IsAssignableFrom(collectionType)) {
				var keyType = ReadTypeHeader(reader);
				var valType = ReadTypeHeader(reader);
				IDictionary dictionary;

				if (collectionType.IsGenericType)
					dictionary = (IDictionary)Activator.CreateInstance(collectionType.MakeGenericType(keyType, valType));
				else
					dictionary = (IDictionary)Activator.CreateInstance(collectionType);

				var count = reader.ReadInt32();
				for (var i = 0; i < count; i++) {
					var itemKeyType = ReadTypeHeader(reader);
					var itemValueType = ReadTypeHeader(reader);
					var key = DeserializeMember(itemKeyType, reader);
					var val = DeserializeMember(itemValueType, reader);
					dictionary.Add(key, val);
				}

				list = dictionary;
			} else {
				var elementType = ReadTypeHeader(reader);
				MethodInfo addMethod;
				if (collectionType.IsGenericType) {
					list = Activator.CreateInstance(collectionType.MakeGenericType(elementType));
					addMethod = list.GetType().GetMethod("Add") ?? throw new InvalidOperationException($"Generic list {collectionType.Name} doesn't support Add method, cannot deserialize");
				} else {
					list = Activator.CreateInstance(collectionType);
					addMethod = collectionType.GetMethod("Add") ?? throw new InvalidOperationException($"list {collectionType.Name} doesn't support Add method, cannot deserialize");
				}

				var count = reader.ReadInt32();

				for (var i = 0; i < count; i++) {
					var itemType = ReadTypeHeader(reader);
					var obj = DeserializeMember(itemType, reader);
					addMethod.Invoke(list, new[] { obj });
				}
			}

			return list;
		}

		private object DeserializeValueType(Type memberType, EndianBinaryReader reader) {
			if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
				memberType = memberType.GetField("value", BindingFlags.NonPublic | BindingFlags.Instance)!.FieldType;
				return DeserializeMember(memberType, reader);
			}
			
			if (memberType == typeof(decimal))
				return reader.ReadDecimal();
			
			if (Serializers.TryGetValue(memberType, out var serializer))
				return serializer.Deserialize(-1, reader);

			var instance = Activator.CreateInstance(memberType);
			foreach (var field in GetSerializableProperties(memberType)) {
				field.FastSetValue(instance, DeserializeMember(field.PropertyType, reader));
			}

			return instance;
		}

		private object DeserializeReferenceType(Type memberType, EndianBinaryReader reader) {
			var valueType = ReadTypeHeader(reader);
			
			if (valueType == typeof(NullValue))
				return null;
			
			if (valueType.IsGenericType) {
				var genericArgs = new List<Type>();
				foreach (var _ in valueType.GetGenericArguments()) {
					genericArgs.Add(ReadTypeHeader(reader));
				}

				valueType = valueType.MakeGenericType(genericArgs.ToArray());
			}
			
			if (Serializers.TryGetValue(valueType, out var serializer))
				return serializer.Deserialize(-1, reader);

			var instance = Activator.CreateInstance(valueType);

			if (memberType.IsInstanceOfType(instance)) {
				foreach (var field in GetSerializableProperties(instance.GetType())) {
					field.FastSetValue(instance, DeserializeMember(field.PropertyType, reader));
				}
				return instance;
			} else
				throw new InvalidOperationException($"Found type {valueType.Name} to deserialize, but target property {memberType.Name} is not assignable from {memberType.Name}");
		}

		private byte[] SerializeMember(object value) {

			if (value is null) {
				return SerializeNullValue();
			}

			var valueType = value.GetType();

			if (valueType.IsPrimitive)
				return SerializePrimitive(value);

			if (valueType == typeof(decimal))
				return _bitConverter.GetBytes((decimal)value);

			if (valueType == typeof(string))
				return SerializeString(value as string);

			if (valueType.IsCollection())
				return SerializeCollectionType(value);

			if (valueType.IsValueType)
				return SerializeValueType(value);

			return SerializeReferenceType(value);
		}

		private byte[] SerializeNullValue() {
			using var memoryStream = new MemoryStream();
			using var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);

			writer.Write(Registrations[typeof(NullValue)]);
			return memoryStream.ToArray();
		}

		private byte[] SerializePrimitive(object boxedPrimitive) {
			return boxedPrimitive switch {
				sbyte x => Array.ConvertAll(new[] { x }, input => (byte)input),
				byte x => new[] { x },
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
				_ => throw new InvalidOperationException("Unknown primitive type")
			};
		}

		private byte[] SerializeReferenceType(object value) {
			using var stream = new MemoryStream();
			using var writer = new EndianBinaryWriter(_bitConverter, stream);
			var type = value.GetType();
			
			if (type.IsGenericType) {
				var genericType = type.GetGenericTypeDefinition();
				var args = type.GetGenericArguments();
				writer.Write(GetTypeIndex(genericType));
				foreach (var arg in args) {
					writer.Write(GetTypeIndex(arg));
				}
			} else {
				writer.Write(GetTypeIndex(type));
			}
			
			if (Serializers.TryGetValue(type, out var serializer))
				serializer.Serialize(value, writer);
			else {
				foreach (var property in GetSerializableProperties(type)) {
					writer.Write(SerializeMember(property.FastGetValue(value)));
				}
			}

			return stream.ToArray();
		}

		private byte[] SerializeValueType(object value) {
			using var stream = new MemoryStream();
			using var writer = new EndianBinaryWriter(_bitConverter, stream);
			var valueType = value.GetType();
			var type = valueType;

			if (type == typeof(decimal))
				return _bitConverter.GetBytes((decimal)value);
			
			if (Serializers.TryGetValue(type, out var serializer))
				serializer.Serialize(value, writer);
			else {
				foreach (var property in GetSerializableProperties(type)) {
					writer.Write(SerializeMember(property.FastGetValue(value)));
				}
			}

			return stream.ToArray();
		}

		private byte[] SerializeString(string value) {
			using var stream = new MemoryStream();
			using var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream);

			if (value is null) {
				writer.Write(GetTypeIndex(typeof(NullValue)));
				return stream.ToArray();
			}
			writer.Write(GetTypeIndex(typeof(string)));
			writer.Write(value);
			return stream.ToArray();
		}

		private byte[] SerializeCollectionType(object value) {
			var listType = value.GetType();

			if (value is IDictionary dictionary) {
				var stream = new MemoryStream();
				var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream);
				var type = dictionary.GetType();

				if (type.IsGenericType) {
					var arguments = dictionary.GetType().GetGenericArguments();
					var keyType = arguments[0];
					var valueType = arguments[1];

					writer.Write(GetTypeIndex(type.GetGenericTypeDefinition()));
					writer.Write(GetTypeIndex(keyType));
					writer.Write(GetTypeIndex(valueType));
				} else {
					writer.Write(GetTypeIndex(type));
					writer.Write(GetTypeIndex(typeof(object)));
					writer.Write(GetTypeIndex(typeof(object)));
				}

				if (Serializers.TryGetValue(listType, out var serializer))
					serializer.Serialize(value, writer);
				else {
					writer.Write(dictionary.Count);
					var enumerator = dictionary.GetEnumerator();

					while (enumerator.MoveNext()) {
						var key = enumerator.Key;
						var val = enumerator.Value;
						writer.Write(GetTypeIndex(key!.GetType()));
						writer.Write(GetTypeIndex(val!.GetType()));
						writer.Write(SerializeMember(key));
						writer.Write(SerializeMember(val));
					}
				}

				return stream.ToArray();
			} else if (value is IEnumerable enumerable) {
				using var headerStream = new MemoryStream();
				using var headerStreamWriter = new EndianBinaryWriter(_bitConverter, headerStream);
				Type elementType;

				if (listType.IsGenericType) {
					var genericTypeDef = listType.GetGenericTypeDefinition();
					var listTypeIndex = GetTypeIndex(genericTypeDef);
					headerStreamWriter.Write(listTypeIndex);
					elementType = listType.GetGenericArguments().First();
				} else if (listType.IsArray) {
					headerStreamWriter.Write(GetTypeIndex(typeof(Array)));
					elementType = listType.GetElementType();
				} else {
					elementType = typeof(object);
					headerStreamWriter.Write(GetTypeIndex(listType));
				}

				headerStreamWriter.Write(GetTypeIndex(elementType));

				using var itemStream = new MemoryStream();
				using var itemStreamWriter = new EndianBinaryWriter(_bitConverter, itemStream);

				if (Serializers.TryGetValue(listType, out var serializer))
					serializer.Serialize(value, itemStreamWriter);
				else {
					var enumerator = enumerable.GetEnumerator();
					var count = 0;

					while (enumerator.MoveNext()) {
						var itemType = enumerator.Current!.GetType();

						if (!itemType.IsClass)
							itemStreamWriter.Write(GetTypeIndex(itemType));

						itemStreamWriter.Write(SerializeMember(enumerator.Current));
						count++;
					}
					headerStreamWriter.Write(count);
				}
				return Tools.Array.Concat<byte>(headerStream.ToArray(), itemStream.ToArray());
			} else
				throw new InvalidOperationException("Could not serialize object as it doesn't implement IEnumerable.");
		}

		private Type ReadTypeHeader(EndianBinaryReader reader) {
			var typeIndex = reader.ReadInt32();

			if (Registrations.Any(x => x.Value == typeIndex)) {
				// Performance, iterating dictionary to match value.
				return Registrations.FirstOrDefault(x => x.Value == typeIndex).Key;
			}
			throw new InvalidOperationException($"Found unknown type index {typeIndex} during deserialization of value for property");
		}

		private PropertyInfo[] GetSerializableProperties(Type type) {
			return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
				.Where(x => x.CanRead && x.CanWrite)
				.ToArray();
		}
	}
}
