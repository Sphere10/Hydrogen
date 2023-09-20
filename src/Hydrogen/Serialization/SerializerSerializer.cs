using Hydrogen.FastReflection;
using System;
using System.Linq;

namespace Hydrogen;

public class SerializerSerializer : ItemSerializer<IItemSerializer>, IAutoSizedSerializer<IItemSerializer> {
	private readonly SizeDescriptorSerializer _typeCodeSerializer;

	public SerializerSerializer(SerializerFactory serializerFactory) {
		SerializerFactory = serializerFactory;
		_typeCodeSerializer = new SizeDescriptorSerializer(SizeDescriptorStrategy.UseCVarInt);
	}

	public SerializerFactory SerializerFactory { get; }

	public override long CalculateSize(IItemSerializer item) {
		Guard.ArgumentNotNull(item, nameof(item));
		var serializerDataType = item.ItemType;
		var serializerHierarchy = SerializerFactory.GetSerializerHierarchy(serializerDataType);
		return serializerHierarchy.Flatten().Sum(_typeCodeSerializer.CalculateSize);
	}

	public override void SerializeInternal(IItemSerializer item, EndianBinaryWriter writer) {
		Guard.ArgumentNotNull(item, nameof(item));
		var serializerDataType = item.ItemType;
		var flattenedHierarchy = SerializerFactory.GetSerializerHierarchy(serializerDataType).Flatten().ToArray();
		foreach(var serializer in flattenedHierarchy)
			_typeCodeSerializer.SerializeInternal(serializer, writer);
	}

	public override IItemSerializer DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		var serializer = Deserialize(reader);
		return serializer;
	}

	public IItemSerializer Deserialize(EndianBinaryReader reader) {
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

}

