﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Sphere10.Framework.FastReflection;

namespace Sphere10.Framework {
	public class GenericItemSerializer<T> : GenericItemSerializer, IItemSerializer<T> where T : new() {
		private EndianBitConverter _bitConverter;

		public bool IsFixedSize => false;

		public int FixedSize => -1;

		private static Lazy<GenericItemSerializer<T>> DefaultInstance { get; } = new(() => new GenericItemSerializer<T>(), LazyThreadSafetyMode.ExecutionAndPublication);

		public static IItemSerializer<T> Default => DefaultInstance.Value;

		public int CalculateTotalSize(IEnumerable<T> items, bool calculateIndividualItems, out int[] itemSizes) {
			throw new NotImplementedException();
		}

		public int CalculateSize(T item) {
			throw new NotImplementedException();
		}


		public bool TrySerialize(T item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				_bitConverter = writer.BitConverter;

				var bytes = SerializeMember(typeof(T), item, new SerializationContext());
				writer.Write(bytes);
				bytesWritten = bytes.Length;
				return true;
			} catch (Exception) {
				bytesWritten = 0;
				return false; 
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out T item) {
			try {
				_bitConverter = reader.BitConverter;
				var context = new SerializationContext();
				item = (T)DeserializeMember(typeof(T), reader, context);
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}

		private object DeserializeMember(Type propertyType, EndianBinaryReader reader, SerializationContext context) {
			var propertyValueType = propertyType;

			if (RequiresTypeHeader(propertyType))
				propertyValueType = ReadTypeHeader(reader);

			if (Serializers.TryGetValue(propertyValueType, out var serializer))
				return serializer.Deserialize(0, reader);

			if (propertyValueType.IsPrimitive)
				return DeserializePrimitive(propertyValueType, reader);

			if (propertyValueType == typeof(string))
				return DeserializeString(reader);

			if (propertyValueType.IsValueType)
				return DeserializeValueType(propertyValueType, reader, context);

			if (propertyValueType.IsCollection())
				return DeserializeCollection(propertyValueType, reader, context);

			return DeserializeReferenceType(propertyValueType, reader, context);
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
			return reader.ReadString();
		}

		private object DeserializeCollection(Type collectionType, EndianBinaryReader reader, SerializationContext context) {
			object list;

			if (Serializers.TryGetValue(collectionType, out var serializer)) {
				return serializer.Deserialize(-1, reader);
			}

			if (typeof(IDictionary).IsAssignableFrom(collectionType)) {
				var dictionary = (IDictionary)Activator.CreateInstance(collectionType);

				var itemKeyType = typeof(object);
				var itemValueType = typeof(object);

				if (collectionType.IsGenericType) {
					var args = collectionType.GetGenericArguments();
					itemKeyType = args[0];
					itemValueType = args[1];
				}

				var count = reader.ReadInt32();
				for (var i = 0; i < count; i++) {
					var key = DeserializeMember(itemKeyType, reader, context);
					var val = DeserializeMember(itemValueType, reader, context);
					dictionary.Add(key, val);
				}

				list = dictionary;
			} else if (collectionType.IsGenericType) {
				list = Activator.CreateInstance(collectionType);
				var addMethod = collectionType.GetMethod("Add") ?? throw new InvalidOperationException($"Generic list {collectionType.Name} doesn't support Add method, cannot deserialize");
				var count = reader.ReadInt32();

				for (var i = 0; i < count; i++) {
					var obj = DeserializeMember(collectionType.GenericTypeArguments[0], reader, context);
					addMethod.Invoke(list, new[] { obj });
				}
			} else if (collectionType.IsArray) {
				var count = reader.ReadInt32();
				var elements = new object[count];
				var elementType = collectionType.GetElementType() ?? typeof(object);

				for (var i = 0; i < count; i++) {
					var obj = DeserializeMember(elementType, reader, context);
					elements[i] = obj;
				}

				var array = Array.CreateInstance(elementType!, elements.Length);
				elements.CopyTo(array, 0);
				list = array;
			} else {
				list = Activator.CreateInstance(collectionType);
				var addMethod = collectionType.GetMethod("Add") ?? throw new InvalidOperationException($"Generic list {collectionType.Name} doesn't support Add method, cannot deserialize");
				var count = reader.ReadInt32();

				for (var i = 0; i < count; i++) {
					var itemType = ReadTypeHeader(reader);
					var obj = DeserializeMember(itemType, reader, context);
					addMethod.Invoke(list, new[] { obj });
				}
			}

			return list;
		}

		private object DeserializeValueType(Type propertyType, EndianBinaryReader reader, SerializationContext context) {
			if (propertyType == typeof(NullValue))
				return null;

			if (propertyType == typeof(CircularReference)) {
				var type = ReadTypeHeader(reader);
				var index = (ushort)DeserializePrimitive(typeof(ushort), reader);
				return context.GetObjectByIndex(type, index);
			}

			if (propertyType.IsNullable()) {
				propertyType = propertyType.GetField("value", BindingFlags.NonPublic | BindingFlags.Instance)!.FieldType;
				return DeserializeMember(propertyType, reader, context);
			}

			if (propertyType == typeof(decimal))
				return reader.ReadDecimal();

			if (Serializers.TryGetValue(propertyType, out var serializer))
				return serializer.Deserialize(-1, reader);

			var instance = Activator.CreateInstance(propertyType);
			foreach (var field in GetSerializableProperties(propertyType)) {
				field.FastSetValue(instance, DeserializeMember(field.PropertyType, reader, context));
			}

			return instance;
		}

		private object DeserializeReferenceType(Type propertyValueType, EndianBinaryReader reader, SerializationContext context) {
			if (Serializers.TryGetValue(propertyValueType, out var serializer))
				return serializer.Deserialize(-1, reader);

			var instance = Activator.CreateInstance(propertyValueType);
			context.AddSerializedObject(instance);

			foreach (var property in GetSerializableProperties(instance.GetType()))
				property.FastSetValue(instance, DeserializeMember(property.PropertyType, reader, context));

			return instance;
		}

		private byte[] SerializeMember(Type propertyType, object value, SerializationContext context) {
			if (value is null)
				return SerializeNullValue();

			var valueType = value.GetType();

			if (IsCircularReference(valueType, value, context, out var reference))
				return SerializeMember(typeof(CircularReference), reference, context);

			var builder = new ByteArrayBuilder();
			if (RequiresTypeHeader(propertyType)) {
				builder.Append(CreateTypeHeader(valueType));
			}

			if (Serializers.TryGetValue(valueType, out var serializer)) {
				var stream = new MemoryStream();
				var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream);
				serializer.Serialize(value, writer);
				builder.Append(stream.ToArray());
			} else {
				if (valueType.IsPrimitive)
					builder.Append(SerializePrimitive(value));
				else if (valueType == typeof(string))
					builder.Append(SerializeString(value as string));
				else if (valueType.IsValueType)
					builder.Append(SerializeValueType(value, context));
				else if (valueType.IsCollection())
					builder.Append(SerializeCollectionType(value, context));
				else
					builder.Append(SerializeReferenceType(value, context));
			}

			return builder.ToArray();
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

		private byte[] SerializeReferenceType(object value, SerializationContext context) {
			using var stream = new MemoryStream();
			using var writer = new EndianBinaryWriter(_bitConverter, stream);
			var type = value.GetType();

			context.AddSerializedObject(value);

			foreach (var property in GetSerializableProperties(type)) {
				writer.Write(SerializeMember(property.PropertyType, property.FastGetValue(value), context));
			}

			return stream.ToArray();
		}

		private byte[] SerializeValueType(object value, SerializationContext context) {
			using var stream = new MemoryStream();
			using var writer = new EndianBinaryWriter(_bitConverter, stream);
			var valueType = value.GetType();

			if (valueType == typeof(decimal))
				return _bitConverter.GetBytes((decimal)value);

			foreach (var property in GetSerializableProperties(valueType)) {
				writer.Write(SerializeMember(property.PropertyType, property.FastGetValue(value), context));
			}

			return stream.ToArray();
		}

		private byte[] SerializeString(string value) {
			using var stream = new MemoryStream();
			using var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream);
			writer.Write(value);
			return stream.ToArray();
		}

		private byte[] SerializeCollectionType(object value, SerializationContext context) {
			var listType = value.GetType();
			
			if (value is IDictionary dictionary) {
				var stream = new MemoryStream();
				var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream);
				writer.Write(dictionary.Count);
				var enumerator = dictionary.GetEnumerator();

				while (enumerator.MoveNext()) {
					var key = enumerator.Key;
					var val = enumerator.Value;

					writer.Write(SerializeMember(key!.GetType(), key, context));
					writer.Write(SerializeMember(val.GetType(), val, context));
				}
				return stream.ToArray();
			} else if (listType.IsGenericType) {
				using var itemStream = new MemoryStream();
				using var itemStreamWriter = new EndianBinaryWriter(_bitConverter, itemStream);
				var count = 0;
				var list = value as IEnumerable;

				//Use generic type arg as element type when deserializing except when more info required.
				var itemType = list!.GetType().GenericTypeArguments[0];
				var enumerator = list.GetEnumerator();
				while (enumerator.MoveNext()) {
					itemStreamWriter.Write(SerializeMember(itemType, enumerator.Current, context));
					count++;
				}

				return Tools.Array.Concat<byte>(SerializePrimitive(count), itemStream.ToArray());
			} else if (value is IEnumerable enumerable) {
				using var itemStream = new MemoryStream();
				using var itemStreamWriter = new EndianBinaryWriter(_bitConverter, itemStream);
				var count = 0;

				var enumerator = enumerable.GetEnumerator();

				while (enumerator.MoveNext()) {
					var itemType = enumerator.Current!.GetType();
					if (!RequiresTypeHeader(itemType))
						itemStreamWriter.Write(GetTypeIndex(itemType));

					itemStreamWriter.Write(SerializeMember(itemType, enumerator.Current, context));
					count++;
				}

				return Tools.Array.Concat<byte>(SerializePrimitive(count), itemStream.ToArray());
			} else
				throw new InvalidOperationException("Could not serialize object as it doesn't implement IEnumerable.");
		}

		private Type ReadTypeHeader(EndianBinaryReader reader) {
			var typeIndex = reader.ReadInt32();

			Type type;
			if (Registrations.Any(x => x.Value == typeIndex)) {
				// Performance, iterating dictionary to match value.
				type = Registrations.Single(x => x.Value == typeIndex).Key;
			} else
				throw new InvalidOperationException($"Unknown type index {typeIndex} found while reading type header");

			if (type.IsGenericType)
				type = type.MakeGenericType(
					type.GetGenericArguments()
						.Select(_ => ReadTypeHeader(reader))
						.ToArray());

			if (type == typeof(Array)) {
				var elementType = ReadTypeHeader(reader);
				type = elementType.MakeArrayType(1);
			}

			return type;
		}

		private PropertyInfo[] GetSerializableProperties(Type type) {
			return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
				.Where(x => x.CanRead && x.CanWrite)
				.ToArray();
		}

		private bool RequiresTypeHeader(Type type) => !type.IsNullable() && type.IsGenericType || type.IsClass || type == typeof(string) || type == typeof(CircularReference);

		private bool IsCircularReference(Type valueType, object value, SerializationContext context, out CircularReference? circularReference) {
			circularReference = default;
			if (valueType.IsClass) {
				if (context.TryGetObjectIndex(value, out var index)) {
					circularReference = new CircularReference {
						Index = (ushort)index,
						TypeIndex = GetTypeIndex(valueType)
					};
					return true;
				} else
					return false;
			} else
				return false;
		}

		private byte[] CreateTypeHeader(Type valueType) {
			var stream = new MemoryStream();
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream);

			if (valueType.IsArray) {
				writer.Write(GetTypeIndex(typeof(Array)));
				writer.Write(GetTypeIndex(valueType.GetElementType()));
			} else
				writer.Write(GetTypeIndex(valueType.IsGenericType ? valueType.GetGenericTypeDefinition() : valueType));

			if (valueType.IsGenericType) {
				foreach (var typeArgument in valueType.GenericTypeArguments) {
					writer.Write(GetTypeIndex(typeArgument));
				}
			}

			return stream.ToArray();
		}
	}
}