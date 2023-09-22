// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Hydrogen.FastReflection;

namespace Hydrogen;

/// <summary>
/// A serializer for any object. It will intelligently serialize it's member fields/properties in a recursive manner and supports
/// circular references between objects.
/// </summary>
/// <typeparam name="T"></typeparam>
public class GenericSerializer<T> : GenericSerializerBase, IItemSerializer<T> {
	
	public bool SupportsNull => true;

	public bool IsConstantSize => false;

	public long ConstantSize => -1;

	/// <summary>
	/// Initialize a new instance of <see cref="GenericSerializerBase"/>. Registers generic type param
	/// in type registrations.
	/// </summary>
	public GenericSerializer() {
		RegisterType<T>();
	}

	private static Lazy<GenericSerializer<T>> DefaultInstance { get; } = new(() => new GenericSerializer<T>(), LazyThreadSafetyMode.ExecutionAndPublication);

	public static IItemSerializer<T> Default => DefaultInstance.Value;

	public long CalculateTotalSize(IEnumerable<T> items, bool calculateIndividualItems, out long[] itemSizes) {

		var itemsArray = items as T[] ?? items.ToArray();
		var sizes = new long[itemsArray.Length];

		for (int i = 0; i < itemsArray.Length; i++)
			sizes[i] = CalculateSize(itemsArray[i]);

		itemSizes = calculateIndividualItems ? sizes : null;
		return sizes.Sum(x => x);
	}

	public long CalculateSize(T item) {
		var context = new SerializationContext();
		SerializeInternal(typeof(T), item, context);

		return context.SizeBytes;
	}

	public void Serialize(T item, EndianBinaryWriter writer) {
		var context = new SerializationContext(writer);
		SerializeInternal(typeof(T), item, context);
	}

	public T Deserialize(EndianBinaryReader reader) {
		var context = new SerializationContext(reader);
		return (T)DeserializeInternal(typeof(T), context);
	}

	private void SerializeInternal(Type propertyType, object propertyValue, SerializationContext context) {
		if (propertyValue is not null) {
			var valueType = propertyValue.GetType();

			if (IsCircularReference(valueType, propertyValue, context, out var reference)) {
				propertyValue = reference;
				valueType = typeof(CircularReference);
			}

			if (RequiresTypeHeader(propertyType))
				CreateTypeHeader(valueType, context);

			if (Serializers.TryGetValue(valueType, out var serializer)) {
				SerializeCustom(serializer, propertyValue, context);
			} else if (valueType.IsEnum)
				SerializePrimitive(Tools.Object.ChangeType(propertyValue, propertyType.GetEnumUnderlyingType()), context);
			else if (valueType.IsPrimitive)
				SerializePrimitive(propertyValue, context);
			else if (valueType.IsValueType)
				SerializeValueType(propertyValue, context);
			else if (valueType.IsCollection())
				SerializeCollectionType(propertyValue, context);
			else
				SerializeReferenceType(propertyValue, context);
		} else
			SerializePrimitive(DetermineTypeCode(typeof(NullValue)), context);
	}

	private void SerializeCustom(IItemSerializer<object> serializer, object propertyValue, SerializationContext context) {
		// Uses a custom serializer, so prefix with byte-sze of what it serializes
		var byteSize = serializer.CalculateSize(propertyValue);
		if (context.IsSizing) {
			context.SizeBytes += CVarInt.SizeOf((ulong)byteSize) + byteSize;
		} else {
			context.Writer.Write(new CVarInt((ulong)byteSize));
			serializer.Serialize(propertyValue, context.Writer);
		}
	}

	private void SerializePrimitive(object boxedPrimitive, SerializationContext context) {
		if (context.IsSizing) {
			var size = boxedPrimitive switch {
				sbyte => sizeof(sbyte),
				byte => sizeof(byte),
				short => sizeof(short),
				ushort primitive => CVarInt.SizeOf(primitive),
				int => sizeof(int),
				uint primitive => CVarInt.SizeOf(primitive),
				long => sizeof(long),
				ulong primitive => CVarInt.SizeOf(primitive),
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
					context.Writer.Write(new CVarInt(x).ToBytes());
					break;
				case int x:
					context.Writer.Write(x);
					break;
				case uint x:
					context.Writer.Write(new CVarInt(x).ToBytes());
					break;
				case long x:
					context.Writer.Write(x);
					break;
				case ulong x:
					context.Writer.Write(new CVarInt(x).ToBytes());
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

		if (value is IDictionary dictionary)
			SerializeDictionary(dictionary, context);
		else if (value is IEnumerable list) {
			var items = new List<object>();
			var enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
				items.Add(enumerator.Current);

			SerializePrimitive(items.Count, context);

			foreach (var item in items) {
				var itemType = listType.IsGenericType ? listType.GenericTypeArguments[0] : item!.GetType();
				SerializeInternal(itemType, item, context);
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

	private object DeserializeInternal(Type propertyType, SerializationContext context) {
		var propertyValueType = propertyType;

		if (RequiresTypeHeader(propertyType))
			propertyValueType = ReadTypeHeader(propertyType, context);

		if (Serializers.TryGetValue(propertyValueType, out var serializer))
			return DeserializeCustom(serializer, context);

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

	private object DeserializeCustom(IItemSerializer<object> serializer, SerializationContext context) {
		return ((IItemSerializer)serializer).Deserialize(context.Reader);
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
			return (ushort)CVarInt.Read(context.Reader.BaseStream);

		if (t == typeof(int))
			return reader.ReadInt32();

		if (t == typeof(uint))
			return (uint)CVarInt.Read(context.Reader.BaseStream);

		if (t == typeof(long))
			return reader.ReadInt64();

		if (t == typeof(ulong))
			return (ulong)CVarInt.Read(context.Reader.BaseStream);

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
				var itemType = collectionType.IsGenericType ? collectionType.GenericTypeArguments[0] : ReadTypeHeader(typeof(object), context);
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
			return ((IItemSerializer)serializer).Deserialize(reader);

		if (propertyType.IsNullable()) {
			propertyType = propertyType.GetField("value", BindingFlags.NonPublic | BindingFlags.Instance)!.FieldType;
			return DeserializeInternal(propertyType, context);
		}

		var instance = Tools.Object.Create(propertyType);
		foreach (var field in GetSerializableProperties(propertyType))
			field.FastSetValue(instance, DeserializeInternal(field.PropertyType, context));

		return instance;
	}

	private object DeserializeCircularReference(SerializationContext context) {
		var referenceType = ReadTypeHeader(typeof(object), context);
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

	private bool RequiresTypeHeader(Type propertyType) {
		return !propertyType.IsNullable()
		       && propertyType.IsGenericType
		       || propertyType.IsClass && !propertyType.IsArray
		       || propertyType == typeof(string)
		       || propertyType == typeof(CircularReference);
	}

	private Type ReadTypeHeader(Type propertyType, SerializationContext context) {
		var serializedTypeCode = context.Reader.ReadInt32();

		Type type;
		if (Registrations.Any(x => x.Value == serializedTypeCode)) {
			// Performance, iterating dictionary to match value.
			type = Registrations.Single(x => x.Value == serializedTypeCode).Key;
		} else {
			// serialized type code is not known - either not serialized by this instance or not yet deserialized.
			// in polymorphic scenarios where the property type does not match the value type then deserialization
			// will fail here as the real type must be pre-registered.
			var knownType = propertyType.IsGenericType ? propertyType.GetGenericTypeDefinition() : propertyType;
			var propertyTypeCode = DetermineTypeCode(knownType);

			if (propertyTypeCode == serializedTypeCode)
				type = knownType;
			else
				throw new InvalidOperationException($"Unknown serialized type code {propertyTypeCode} for property type {propertyType}, register type.");
		}

		if (type.IsGenericType) {

			Type[] genericArgs;
			if (propertyType.IsGenericType)
				genericArgs = propertyType.GetGenericArguments()
					.Select(x => ReadTypeHeader(x, context))
					.ToArray();
			else {
				// property value is generic, but property type is not (e.g. object). See if generic param type is registered.
				genericArgs = type.GetGenericArguments()
					.Select(x => ReadTypeHeader(typeof(object), context))
					.ToArray();
			}

			type = type.MakeGenericType(genericArgs);
		}


		if (type == typeof(Array)) {
			var elementType = ReadTypeHeader(propertyType.GetElementType(), context);
			type = elementType.MakeArrayType();
		}

		return type;
	}

	private void CreateTypeHeader(Type valueType, SerializationContext context) {
		if (valueType.IsArray) {
			if (valueType.GetArrayRank() > 1)
				throw new InvalidOperationException("Multi-dimension arrays are not supported.");

			SerializePrimitive(DetermineTypeCode(typeof(Array)), context);
			SerializePrimitive(DetermineTypeCode(valueType.GetElementType()), context);

		} else
			SerializePrimitive(DetermineTypeCode(valueType.IsGenericType ? valueType.GetGenericTypeDefinition() : valueType), context);

		if (valueType.IsGenericType) {
			foreach (var typeArgument in valueType.GenericTypeArguments) {
				SerializePrimitive(DetermineTypeCode(typeArgument), context);
			}
		}
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
					TypeCode = DetermineTypeCode(valueType)
				};
				return true;
			} else
				return false;
		} else
			return false;
	}


	private class SerializationContext {

		/// <summary>
		/// backing field for length property used when calculating item size.
		/// </summary>
		private long _size;

		/// <summary>
		/// the writer's inner stream start position used to determine how many bytes have been written.
		/// </summary>
		private readonly long _startPosition;

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
