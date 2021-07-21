using System;
using System.Collections;
using System.Collections.Generic;
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
			var context = new SerializationContext();
			SerializeInternal(typeof(T), item, context);

			return (int)context.SizeBytes;
		}

		public bool TrySerialize(T item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				var context = new SerializationContext(writer);
				SerializeInternal(typeof(T), item, context);
				bytesWritten = (int)context.SizeBytes;

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

			if (propertyType.IsEnum)
				return DeserializeEnum(propertyValueType, context);

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

		private object DeserializeEnum(Type propertyValueType, SerializationContext context) {
			var enumUnderlyingType = Enum.GetUnderlyingType(propertyValueType);
			var value = DeserializePrimitive(enumUnderlyingType, context);
			return Enum.ToObject(propertyValueType, value);
		}

		private void SerializeInternal(Type propertyType, object propertyValue, SerializationContext context) {
			if (propertyValue is null)
				SerializePrimitive(Registrations[typeof(NullValue)], context);
			else {
				var valueType = propertyValue.GetType();

				if (IsCircularReference(valueType, propertyValue, context, out var reference)) {
					propertyValue = reference;
					valueType = typeof(CircularReference);
				}

				if (RequiresTypeHeader(propertyType))
					CreateTypeHeader(valueType, context);

				if (Serializers.TryGetValue(valueType, out var serializer)) {
					if (context.IsSizing)
						context.SizeBytes += serializer.CalculateSize(propertyValue);
					else
						serializer.Serialize(propertyValue, context.Writer);
				} else if (valueType.IsEnum)
					SerializePrimitive(Convert.ChangeType(propertyValue, propertyType.GetEnumUnderlyingType()), context);
				else if (valueType.IsPrimitive)
					SerializePrimitive(propertyValue, context);
				else if (valueType.IsValueType)
					SerializeValueType(propertyValue, context);
				else if (valueType.IsCollection())
					SerializeCollectionType(propertyValue, context);
				else
					SerializeReferenceType(propertyValue, context);

			}
		}

		private void SerializePrimitive(object boxedPrimitive, SerializationContext context) {
			if (context.IsSizing) {
				int size = boxedPrimitive switch {
					sbyte => sizeof(sbyte),
					byte => sizeof(byte),
					short => sizeof(short),
					ushort => sizeof(ushort),
					int => sizeof(int),
					uint => sizeof(uint),
					long => sizeof(long),
					ulong => sizeof(ulong),
					float => sizeof(float),
					double => sizeof(double),
					decimal => sizeof(decimal),
					bool => sizeof(bool),
					char => sizeof(char),
					_ => throw new InvalidOperationException("Unknown primitive type")
				};
				context.SizeBytes += size;
			} else {
				switch (boxedPrimitive) {
					case sbyte x:
						context.Writer.Write(x);
						break;
					case byte x:
						context.Writer.Write(x);
						break;
					case short x:
						context.Writer.Write(x);
						break;
					case ushort x:
						context.Writer.Write(x);
						break;
					case int x:
						context.Writer.Write(x);
						break;
					case uint x:
						context.Writer.Write(x);
						break;
					case long x:
						context.Writer.Write(x);
						break;
					case ulong x:
						context.Writer.Write(x);
						break;
					case float x:
						context.Writer.Write(x);
						break;
					case double x:
						context.Writer.Write(x);
						break;
					case decimal x:
						context.Writer.Write(x);
						break;
					case bool x:
						context.Writer.Write(x);
						break;
					case char x:
						context.Writer.Write(x);
						break;
					default:
						throw new InvalidOperationException("Unknown primitive type");
				}
			}
		}

		private void SerializeReferenceType(object value, SerializationContext context) {
			var type = value.GetType();
			context.AddSerializedObject(value);
			foreach (var property in GetSerializableProperties(type)) {
				SerializeInternal(property.PropertyType, property.FastGetValue(value), context);
			}
		}

		private void SerializeValueType(object value, SerializationContext context) {
			var valueType = value.GetType();
			foreach (var property in GetSerializableProperties(valueType)) {
				SerializeInternal(property.PropertyType, property.FastGetValue(value), context);
			}
		}

		private void SerializeCollectionType(object value, SerializationContext context) {
			var listType = value.GetType();

			if (value is IDictionary dictionary) {
				SerializeDictionary(dictionary, context);
			} else if (value is IEnumerable list) {
				var enumerator = list.GetEnumerator();

				int count = 0;
				while (enumerator.MoveNext())
					count++;

				SerializePrimitive(count, context);

				enumerator.Reset();
				while (enumerator.MoveNext()) {
					var itemType = listType.IsGenericType ? listType.GenericTypeArguments[0] : enumerator.Current!.GetType();
					SerializeInternal(itemType, enumerator.Current, context);
				}
			} else
				throw new InvalidOperationException("Could not serialize object as it doesn't implement IEnumerable.");
		}

		private void SerializeDictionary(IDictionary dictionary, SerializationContext context) {
			SerializePrimitive(dictionary.Count, context);

			var enumerator = dictionary.GetEnumerator();

			while (enumerator.MoveNext()) {
				var key = enumerator.Key;
				var val = enumerator.Value;

				SerializeInternal(key!.GetType(), key, context);
				SerializeInternal(val.GetType(), val, context);
			}
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

		/// <summary>
		/// Determines whether <paramref name="value"/> is a reference to an object that has already been
		/// seen during serialization. If so, out value contains a CircularReference struct to be serialized instead.
		/// returns false if object has not been serialized already.
		/// </summary>
		/// <param name="valueType"></param>
		/// <param name="value"></param>
		/// <param name="context"></param>
		/// <param name="circularReference"></param>
		/// <returns></returns>
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

		private void CreateTypeHeader(Type valueType, SerializationContext context) {
			if (valueType.IsArray) {
				if (valueType.GetArrayRank() > 1)
					throw new InvalidOperationException("Multi-dimension arrays are not supported.");

				SerializePrimitive(GetTypeIndex(typeof(Array)), context);
				SerializePrimitive(GetTypeIndex(valueType.GetElementType()), context);

			} else
				SerializePrimitive(GetTypeIndex(valueType.IsGenericType ? valueType.GetGenericTypeDefinition() : valueType), context);

			if (valueType.IsGenericType) {
				foreach (var typeArgument in valueType.GenericTypeArguments) {
					SerializePrimitive(GetTypeIndex(typeArgument), context);
				}
			}
		}


		private class SerializationContext {

			/// <summary>
			/// backing field for length property used when calculating item size.
			/// </summary>
			private long _size;

			/// <summary>
			/// the writer's inner stream start position used to determine how many bytes have been written.
			/// </summary>
			private long _startPosition;

			/// <summary>
			/// ref dictionary.
			/// </summary>
			private readonly Dictionary<Reference<object>, int> _referenceDictionary = new();

			/// <summary>
			/// Init new context with reader for deserialization.
			/// </summary>
			/// <param name="reader"></param>
			/// <exception cref="ArgumentNullException"></exception>
			public SerializationContext(EndianBinaryReader reader) {
				Reader = reader ?? throw new ArgumentNullException(nameof(reader));
			}

			/// <summary>
			/// Init new context with writer for serialization.
			/// </summary>
			/// <param name="writer"></param>
			/// <exception cref="ArgumentNullException"></exception>
			public SerializationContext(EndianBinaryWriter writer) {
				Writer = writer ?? throw new ArgumentNullException(nameof(writer));
				_startPosition = writer.BaseStream.Position;
			}

			/// <summary>
			/// Init without reader/writer, count bytes only.
			/// </summary>
			public SerializationContext() {
			}

			/// <summary>
			/// Gets the stream writer, null during deserialization.
			/// </summary>
			public EndianBinaryWriter Writer { get; }

			/// <summary>
			/// Gets the stream reader, null during serialization. 
			/// </summary>
			public EndianBinaryReader Reader { get; }

			/// <summary>
			/// Gets the bit converter for the supplied reader/writer.
			/// </summary>
			public EndianBitConverter BitConverter => Writer.BitConverter ?? Reader.BitConverter;

			/// <summary>
			/// Gets a value indicating whether this serialization context is for sizing only.
			/// </summary>
			public bool IsSizing => Writer is null && Reader is null;

			/// <summary>
			/// Gets the size of the item being serialized in bytes.
			/// </summary>
			public long SizeBytes {
				get {
					if (IsSizing)
						return _size;
					else
						return Writer?.BaseStream.Position - _startPosition ?? 0;
				}
				set {
					if (IsSizing)
						_size = value;
					else
						throw new InvalidOperationException("Setting size is not supported during serialization");
				}
			}


			/// <summary>
			/// Retrieve an object stored in the reference dictionary. Objects that have already been serialized or deserialized are in the reference
			/// dictionary and may be resolved by their type and their logical index for use in circular reference scenarios.
			/// </summary>
			/// <param name="type"> the object type</param>
			/// <param name="index"> the objects index, the order its been added</param>
			/// <returns></returns>
			/// <exception cref="InvalidOperationException"> thrown if no object of the supplied type with the supplied index is stored in the ref dict.</exception>
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
