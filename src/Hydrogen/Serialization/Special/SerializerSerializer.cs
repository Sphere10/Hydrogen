using Hydrogen.FastReflection;
using System;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Used to serialize a reference to serializer in a <see cref="SerializerFactory"/>. Used predominantly by <see cref="PolymorphicSerializer{TBase}"/>
/// as a prefix for serialized objects so that deserialization knows which <see cref="IItemSerializer{TItem}"/> to use.
/// </summary>
internal class SerializerSerializer : ItemSerializerBase<IItemSerializer>{
	private readonly SizeDescriptorSerializer _sizeSerializer;

	public SerializerSerializer(SerializerFactory serializerFactory) {
		SerializerFactory = serializerFactory;
		_sizeSerializer = new SizeDescriptorSerializer(SizeDescriptorStrategy.UseCVarInt);
	}

	public SerializerFactory SerializerFactory { get; }

	public override long CalculateSize(SerializationContext context, IItemSerializer item) {
		Guard.ArgumentNotNull(item, nameof(item));
		var factory = context.HasEphemeralFactory ? context.EphemeralFactory : SerializerFactory;
		var serializerDataType = item.ItemType;
		var serializerHierarchy = factory.GetSerializerHierarchy(serializerDataType);
		return serializerHierarchy.Flatten().Sum(item1 => _sizeSerializer.CalculateSize(context, item1));
	}

	public override void Serialize(IItemSerializer item, EndianBinaryWriter writer, SerializationContext context) {
		Guard.ArgumentNotNull(item, nameof(item));
		var factory = context.HasEphemeralFactory ? context.EphemeralFactory : SerializerFactory;
		var serializerDataType = item.ItemType;
		var flattenedHierarchy = factory.GetSerializerHierarchy(serializerDataType).Flatten().ToArray();
		foreach(var serializer in flattenedHierarchy)
			_sizeSerializer.Serialize(serializer, writer, context);
	}

	public override IItemSerializer Deserialize(EndianBinaryReader reader, SerializationContext context) {
		// deserialize the top-level serializer code
		var factory = context.HasEphemeralFactory ? context.EphemeralFactory : SerializerFactory;
		var rootSerializerCode = _sizeSerializer.Deserialize(reader, context);
		var serializerHierarchy = RecursiveDataType<long>.Parse(
			rootSerializerCode, 
			factory.CountSubSerializers, 
			() => _sizeSerializer.Deserialize(reader, context)
		);
		var rootSerializer = factory.FromSerializerHierarchy(serializerHierarchy);
		return rootSerializer;
	}

}
