namespace Hydrogen;

public sealed class ObjectSerializer : FactorySerializer<object> {

	public ObjectSerializer() 
		: this (SerializerFactory.Default) {
	}

	public ObjectSerializer(SerializerFactory factory) 
		: base(factory) {
	}

}