namespace Hydrogen;

/// <summary>
/// Replacement for BinarySerializer deprecated in .NET 5.0 using Hydrogen's serialization framework.
/// </summary>
public sealed class BinarySerializer : ItemSerializerDecorator<object> {
	public BinarySerializer() : this(SerializerFactory.Default) {
	}

	public BinarySerializer(SerializerFactory serializerFactory) 
		: base(new PolymorphicSerializer<object>(serializerFactory)) {
	}

}
