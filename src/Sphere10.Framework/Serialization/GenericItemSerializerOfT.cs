using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Sphere10.Framework.FastReflection;

namespace Sphere10.Framework {
	public class GenericItemSerializer<T> : GenericItemSerializer, IItemSerializer<T> where T : new() {
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
				var bytes = SerializeInternal(typeof(T), item, new SerializationContext(writer));
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
				var context = new SerializationContext(reader);
				item = (T)DeserializeInternal(typeof(T), context);
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}

		private object DeserializeInternal(Type propertyType, SerializationContext context) {
			var propertyValueType = propertyType;

			if (RequiresTypeHeader(propertyType))
				propertyValueType = ReadTypeHeader(context);

			if (Serializers.TryGetValue(propertyValueType, out var serializer))
				return serializer.Deserialize(0, context.Reader);

			if (propertyValueType.IsPrimitive)
				return DeserializePrimitive(propertyValueType, context);

			if (propertyValueType == typeof(string))
				return DeserializeString(context);

			if (propertyValueType.IsValueType)
				return DeserializeValueType(propertyValueType, context);

			if (propertyValueType.IsCollection())
				return DeserializeCollection(propertyValueType, context);

			return DeserializeReferenceType(propertyValueType, context);
		}

		private object DeserializePrimitive(Type t, SerializationContext context) {

			var reader = context.Reader;
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

		private string DeserializeString(SerializationContext context) => context.Reader.ReadString();

		private object DeserializeCollection(Type collectionType, SerializationContext context) {
			object list;
			var reader = context.Reader;

			if (typeof(IDictionary).IsAssignableFrom(collectionType))
				list = DeserializeDictionary(collectionType, context);
			else if (collectionType.IsArray)
				list = DeserializeArray(collectionType, context);
			else {
				list = Tools.Object.Create(collectionType);
				var addMethod = collectionType.GetMethod("Add") ?? throw new InvalidOperationException($"Generic list {collectionType.Name} doesn't support Add method, cannot deserialize");
				var count = reader.ReadInt32();

				for (var i = 0; i < count; i++) {
					var itemType = collectionType.IsGenericType ? collectionType.GenericTypeArguments[0] : ReadTypeHeader(context);
					var obj = DeserializeInternal(itemType, context);
					addMethod.FastInvoke(list, new[] { obj });
				}
			}

			return list;
		}
		private Array DeserializeArray(Type collectionType, SerializationContext context) {
			var count = context.Reader.ReadInt32();
			var elements = new object[count];
			var elementType = collectionType.GetElementType() ?? typeof(object);

			for (var i = 0; i < count; i++) {
				var obj = DeserializeInternal(elementType, context);
				elements[i] = obj;
			}

			var array = Array.CreateInstance(elementType!, elements.Length);
			elements.CopyTo(array, 0);
			return array;
		}

		private IDictionary DeserializeDictionary(Type collectionType, SerializationContext context) {
			var dictionary = (IDictionary)Tools.Object.Create(collectionType);

			var itemKeyType = typeof(object);
			var itemValueType = typeof(object);

			if (collectionType.IsGenericType) {
				var args = collectionType.GetGenericArguments();
				itemKeyType = args[0];
				itemValueType = args[1];
			}

			var count = context.Reader.ReadInt32();
			for (var i = 0; i < count; i++) {
				var key = DeserializeInternal(itemKeyType, context);
				var val = DeserializeInternal(itemValueType, context);
				dictionary.Add(key, val);
			}

			return dictionary;
		}

		private object DeserializeValueType(Type propertyType, SerializationContext context) {
			var reader = context.Reader;

			if (propertyType == typeof(NullValue))
				return null;

			if (propertyType == typeof(CircularReference))
				return DeserializeCircularReference(context);

			if (Serializers.TryGetValue(propertyType, out var serializer))
				return serializer.Deserialize(-1, reader);

			if (propertyType.IsNullable()) {
				propertyType = propertyType.GetField("value", BindingFlags.NonPublic | BindingFlags.Instance)!.FieldType;
				return DeserializeInternal(propertyType, context);
			}

			var instance = Tools.Object.Create(propertyType);
			foreach (var field in GetSerializableProperties(propertyType)) {
				field.FastSetValue(instance, DeserializeInternal(field.PropertyType, context));
			}

			return instance;
		}
		private object DeserializeCircularReference(SerializationContext context) {
			var referenceType = ReadTypeHeader(context);
			var index = (ushort)DeserializePrimitive(typeof(ushort), context);
			return context.GetObjectByIndex(referenceType, index);
		}

		private object DeserializeReferenceType(Type propertyValueType, SerializationContext context) {
			var instance = Tools.Object.Create(propertyValueType);
			context.AddSerializedObject(instance);

			foreach (var property in GetSerializableProperties(instance.GetType()))
				property.FastSetValue(instance, DeserializeInternal(property.PropertyType, context));

			return instance;
		}

		private byte[] SerializeInternal(Type propertyType, object value, SerializationContext context) {
			if (value is null)
				return SerializeNullValue();

			var valueType = value.GetType();

			if (IsCircularReference(valueType, value, context, out var reference))
				return SerializeInternal(typeof(CircularReference), reference, context);

			var builder = new ByteArrayBuilder();
			if (RequiresTypeHeader(propertyType)) {
				builder.Append(CreateTypeHeader(valueType));
			}

			if (Serializers.TryGetValue(valueType, out var serializer)) {
				using var stream = new MemoryStream();
				using var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream);
				serializer.Serialize(value, writer);
				return stream.ToArray();
			}

			if (valueType.IsPrimitive)
				builder.Append(SerializePrimitive(value, context));
			else if (valueType == typeof(string))
				builder.Append(SerializeString(value as string));
			else if (valueType.IsValueType)
				builder.Append(SerializeValueType(value, context));
			else if (valueType.IsCollection())
				builder.Append(SerializeCollectionType(value, context));
			else
				builder.Append(SerializeReferenceType(value, context));

			return builder.ToArray();
		}


		private byte[] SerializeNullValue() {
			using var memoryStream = new MemoryStream();
			using var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
			writer.Write(Registrations[typeof(NullValue)]);
			return memoryStream.ToArray();
		}

		private byte[] SerializePrimitive(object boxedPrimitive, SerializationContext context) {
			return boxedPrimitive switch {
				sbyte x => Array.ConvertAll(new[] { x }, input => (byte)input),
				byte x => new[] { x },
				short x => context.BitConverter.GetBytes(x),
				ushort x => context.BitConverter.GetBytes(x),
				int x => context.BitConverter.GetBytes(x),
				uint x => context.BitConverter.GetBytes(x),
				long x => context.BitConverter.GetBytes(x),
				ulong x => context.BitConverter.GetBytes(x),
				float x => context.BitConverter.GetBytes(x),
				double x => context.BitConverter.GetBytes(x),
				decimal x => context.BitConverter.GetBytes(x),
				bool x => context.BitConverter.GetBytes(x),
				char x => context.BitConverter.GetBytes(x),
				_ => throw new InvalidOperationException("Unknown primitive type")
			};
		}

		private byte[] SerializeReferenceType(object value, SerializationContext context) {
			using var stream = new MemoryStream();
			using var writer = new EndianBinaryWriter(context.BitConverter, stream);
			var type = value.GetType();

			context.AddSerializedObject(value);

			foreach (var property in GetSerializableProperties(type)) {
				writer.Write(SerializeInternal(property.PropertyType, property.FastGetValue(value), context));
			}

			return stream.ToArray();
		}

		private byte[] SerializeValueType(object value, SerializationContext context) {
			using var stream = new MemoryStream();
			using var writer = new EndianBinaryWriter(context.BitConverter, stream);
			var valueType = value.GetType();

			foreach (var property in GetSerializableProperties(valueType)) {
				writer.Write(SerializeInternal(property.PropertyType, property.FastGetValue(value), context));
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
				return SerializeDictionary(dictionary, context);
			}
			
			if (value is IEnumerable list) {
				using var itemStream = new MemoryStream();
				using var itemStreamWriter = new EndianBinaryWriter(context.BitConverter, itemStream);
				var count = 0;
				var enumerator = list.GetEnumerator();
				while (enumerator.MoveNext()) {
					var itemType = listType.IsGenericType ? listType.GenericTypeArguments[0] : enumerator.Current!.GetType();
					itemStreamWriter.Write(SerializeInternal(itemType, enumerator.Current, context));
					count++;
				}

				return Tools.Array.Concat<byte>(SerializePrimitive(count, context), itemStream.ToArray());
			} else
				throw new InvalidOperationException("Could not serialize object as it doesn't implement IEnumerable.");
		}
		private byte[] SerializeDictionary(IDictionary dictionary, SerializationContext context) {

			var stream = new MemoryStream();
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream);
			writer.Write(dictionary.Count);
			var enumerator = dictionary.GetEnumerator();

			while (enumerator.MoveNext()) {
				var key = enumerator.Key;
				var val = enumerator.Value;

				writer.Write(SerializeInternal(key!.GetType(), key, context));
				writer.Write(SerializeInternal(val.GetType(), val, context));
			}
			return stream.ToArray();
		}

		private Type ReadTypeHeader(SerializationContext context) {
			var typeIndex = context.Reader.ReadInt32();

			Type type;
			if (Registrations.Any(x => x.Value == typeIndex)) {
				// Performance, iterating dictionary to match value.
				type = Registrations.Single(x => x.Value == typeIndex).Key;
			} else
				throw new InvalidOperationException($"Unknown type index {typeIndex} found while reading type header");

			if (type.IsGenericType)
				type = type.MakeGenericType(
					type.GetGenericArguments()
						.Select(_ => ReadTypeHeader(context))
						.ToArray());

			if (type == typeof(Array)) {
				var elementType = ReadTypeHeader(context);
				type = elementType.MakeArrayType(1);
			}

			return type;
		}

		private PropertyInfo[] GetSerializableProperties(Type type) {
			return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
				.Where(x => x.CanRead && x.CanWrite)
				.ToArray();
		}

		private bool RequiresTypeHeader(Type propertyType) {
			return !propertyType.IsNullable()
			       && propertyType.IsGenericType
			       || propertyType.IsClass
			       || propertyType == typeof(string)
			       || propertyType == typeof(CircularReference);
		}

		private bool IsCircularReference(Type valueType, object value, SerializationContext context, out CircularReference? circularReference) {
			circularReference = default;
			if (valueType.IsClass) {
				if (context.TryGetObjectRefIndex(value, out var index)) {
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
				if (valueType.GetArrayRank() > 1)
					throw new InvalidOperationException("Multi-dimension arrays are not supported.");

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


		private class SerializationContext {

			private readonly Dictionary<Reference<object>, int> _referenceDictionary = new();

			public SerializationContext(EndianBinaryReader reader) {
				Reader = reader ?? throw new ArgumentNullException(nameof(reader));
			}

			public SerializationContext(EndianBinaryWriter writer) {
				Writer = writer ?? throw new ArgumentNullException(nameof(writer));
			}

			public EndianBinaryWriter Writer { get; }

			public EndianBinaryReader Reader { get; }

			public EndianBitConverter BitConverter => Writer.BitConverter ?? Reader.BitConverter;

			public object GetObjectByIndex(Type type, int index) {
				return _referenceDictionary.FirstOrDefault(x => x.Key.Object.GetType() == type && x.Value == index).Key.Object
				       ?? throw new InvalidOperationException($"No reference stored for type {type.Name} with index {index}");
			}

			public void AddSerializedObject(object obj) {
				int index = _referenceDictionary.Count(x => x.Key.Object.GetType() == obj.GetType());
				_referenceDictionary.Add(Reference.For(obj), index);
			}

			public bool TryGetObjectRefIndex(object obj, out int index) => _referenceDictionary.TryGetValue(Reference.For(obj), out index);
		}
	}
}
