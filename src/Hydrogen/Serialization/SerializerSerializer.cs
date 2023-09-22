using Hydrogen.FastReflection;
using System;
using System.Linq;

namespace Hydrogen;

public class SerializerSerializer : ItemSerializer<IItemSerializer>{

	public SerializerSerializer(SerializerFactory serializerFactory) 
		: base(SizeDescriptorStrategy.UseCVarInt) {
		SerializerFactory = serializerFactory;
	}

	public SerializerFactory SerializerFactory { get; }

	public override long CalculateSize(IItemSerializer item) {
		Guard.ArgumentNotNull(item, nameof(item));
		var serializerDataType = item.ItemType;
		var serializerHierarchy = SerializerFactory.GetSerializerHierarchy(serializerDataType);
		return serializerHierarchy.Flatten().Sum(SizeSerializer.CalculateSize);
	}

	public override void SerializeInternal(IItemSerializer item, EndianBinaryWriter writer) {
		Guard.ArgumentNotNull(item, nameof(item));
		var serializerDataType = item.ItemType;
		var flattenedHierarchy = SerializerFactory.GetSerializerHierarchy(serializerDataType).Flatten().ToArray();
		foreach(var serializer in flattenedHierarchy)
			SizeSerializer.SerializeInternal(serializer, writer);
	}

	public override IItemSerializer DeserializeInternal(EndianBinaryReader reader) {
		// deserialize the top-level serializer code
		var rootSerializerCode = SizeSerializer.Deserialize(reader);
		var serializerHierarchy = RecursiveDataType<long>.Parse(
			rootSerializerCode, 
			SerializerFactory.CountSubSerializers, 
			() => SizeSerializer.Deserialize(reader)
		);
		var rootSerializer = SerializerFactory.FromSerializerHierarchy(serializerHierarchy);
		return rootSerializer;
	}

}

