namespace Hydrogen;

public sealed class ObjectSerializer : ItemSerializerDecorator<object> {

	public ObjectSerializer(SerializerFactory factory)
		: base(new ReferenceSerializer<object>(new PolymorphicSerializer<object>( factory, factory.GetPureSerializer<object>()))) {
	}
}


