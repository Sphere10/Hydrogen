using Hydrogen.FastReflection;
using System;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Used to serialize serializers.
/// </summary>
internal class SerializerSerializer : ItemSerializer<IItemSerializer>{

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

	public override void Serialize(IItemSerializer item, EndianBinaryWriter writer) {
		Guard.ArgumentNotNull(item, nameof(item));
		var serializerDataType = item.ItemType;
		var flattenedHierarchy = SerializerFactory.GetSerializerHierarchy(serializerDataType).Flatten().ToArray();
		foreach(var serializer in flattenedHierarchy)
			SizeSerializer.Serialize(serializer, writer);
	}

	public override IItemSerializer Deserialize(EndianBinaryReader reader) {
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

public class EnvelopedSerializer<T> : ItemSerializer<T> {
	public override long CalculateSize(T item) {
		throw new NotImplementedException();
	}

	public override void Serialize(T item, EndianBinaryWriter writer) {
		throw new NotImplementedException();
	}

	public override T Deserialize(EndianBinaryReader reader) {
		throw new NotImplementedException();
	}
}