namespace Hydrogen;

public sealed class ObjectSerializer : PolymorphicSerializer<object> {

	public ObjectSerializer() 
		: this (SerializerFactory.Default) {
	}

	public ObjectSerializer(SerializerFactory factory) 
		: base(factory) {
	}

}