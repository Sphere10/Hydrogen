using Hydrogen.FastReflection;
using System;
using System.Linq;

namespace Hydrogen;

public class SerializerSerializer : ItemSerializer<object>, IAutoSizedSerializer<object> {
	private readonly SizeDescriptorSerializer _typeCodeSerializer;

	public SerializerSerializer(SerializerFactory serializerFactory) {
		SerializerFactory = serializerFactory;
		_typeCodeSerializer = new SizeDescriptorSerializer(SizeDescriptorStrategy.UseCVarInt);
	}

	public SerializerFactory SerializerFactory { get; }

	public override long CalculateSize(object item) {
		Guard.ArgumentNotNull(item, nameof(item));
		Guard.Argument(item.GetType().IsSubtypeOfGenericType(typeof(IItemSerializer<>), out var constructedSubType), nameof(item), $"Must be an {typeof(IItemSerializer<>).ToStringCS()}");
		var serializerDataType = constructedSubType.GenericTypeArguments[0];
		var serializerHierarchy = SerializerFactory.GetSerializerHierarchy(serializerDataType);
		return serializerHierarchy.Flatten().Sum(_typeCodeSerializer.CalculateSize);
	}

	public override void SerializeInternal(object item, EndianBinaryWriter writer) {
		Guard.ArgumentNotNull(item, nameof(item));
		Guard.Argument(item.GetType().IsSubtypeOfGenericType(typeof(IItemSerializer<>), out var constructedSubType), nameof(item), $"Must be an {typeof(IItemSerializer<>).ToStringCS()}");
		var serializerDataType = constructedSubType.GenericTypeArguments[0];
		var flattenedHierarchy = SerializerFactory.GetSerializerHierarchy(serializerDataType).Flatten().ToArray();
		foreach(var serializer in flattenedHierarchy)
			_typeCodeSerializer.SerializeInternal(serializer, writer);
	}

	public override object DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		var serializer = Deserialize(reader);
		Guard.Ensure(serializer.GetType().IsSubtypeOfGenericType(typeof(IItemSerializer<>)), $"Deserialized an object that was not an {typeof(IItemSerializer<>).ToStringCS()}");
		return serializer;
	}

	public object Deserialize(EndianBinaryReader reader) {
		// deserialize the top-level serializer code
		var rootSerializerCode = _typeCodeSerializer.Deserialize(reader);
		var serializerHierarchy = RecursiveDataType<long>.Parse(
			rootSerializerCode, 
			SerializerFactory.CountSubSerializers, 
			() => _typeCodeSerializer.Deserialize(reader)
		);
		var rootSerializer = SerializerFactory.FromSerializerHierarchy(serializerHierarchy);
		return rootSerializer;
	}

		
	public IItemSerializer<TSerializerDataType> GetTypedSerializer<TSerializerDataType>(object serializerObj) {
		if (serializerObj is IItemSerializer<TSerializerDataType> serializer)
			return serializer;

		if (!serializerObj.GetType().IsSubtypeOfGenericType(typeof(IItemSerializer<>), out var actualDataType)) {
			throw new InvalidOperationException($"Serializer object is not an {typeof(IItemSerializer<>).ToStringCS()}");
		}
		actualDataType = actualDataType.GetGenericArguments()[0];

		Guard.Ensure(actualDataType.FastIsSubTypeOf(typeof(TSerializerDataType)), $"Serializer object is not an {typeof(IItemSerializer<>).ToStringCS()}<{typeof(TSerializerDataType).ToStringCS()}>");

		var genericMethod = DecoratorExtensions.SerializerCastMethod.MakeGenericMethod(new [] { actualDataType,  typeof(TSerializerDataType) });
		serializer = genericMethod.FastInvoke(null, new [] { serializerObj }) as IItemSerializer<TSerializerDataType>;;
		return serializer;

	}
}

