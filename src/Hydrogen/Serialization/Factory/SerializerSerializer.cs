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

	public override long CalculateSize(SerializationContext context, IItemSerializer item) {
		Guard.ArgumentNotNull(item, nameof(item));
		var serializerDataType = item.ItemType;
		var serializerHierarchy = SerializerFactory.GetSerializerHierarchy(serializerDataType);
		return serializerHierarchy.Flatten().Sum(item1 => SizeSerializer.CalculateSize(context, item1));
	}

	public override void Serialize(IItemSerializer item, EndianBinaryWriter writer, SerializationContext context) {
		Guard.ArgumentNotNull(item, nameof(item));
		var serializerDataType = item.ItemType;
		var flattenedHierarchy = SerializerFactory.GetSerializerHierarchy(serializerDataType).Flatten().ToArray();
		foreach(var serializer in flattenedHierarchy)
			SizeSerializer.Serialize(serializer, writer, context);
	}

	public override IItemSerializer Deserialize(EndianBinaryReader reader, SerializationContext context) {
		// deserialize the top-level serializer code
		var rootSerializerCode = SizeSerializer.Deserialize(reader, context);
		var serializerHierarchy = RecursiveDataType<long>.Parse(
			rootSerializerCode, 
			SerializerFactory.CountSubSerializers, 
			() => SizeSerializer.Deserialize(reader, context)
		);
		var rootSerializer = SerializerFactory.FromSerializerHierarchy(serializerHierarchy);
		return rootSerializer;
	}

}

public class EnvelopedSerializer<T> : ItemSerializer<T> {
	public override long CalculateSize(SerializationContext context, T item) {
		throw new NotImplementedException();
	}

	public override void Serialize(T item, EndianBinaryWriter writer, SerializationContext context) {
		throw new NotImplementedException();
	}

	public override T Deserialize(EndianBinaryReader reader, SerializationContext context) {
		throw new NotImplementedException();
	}
}