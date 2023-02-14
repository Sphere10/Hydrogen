namespace Hydrogen.Data;

public class SerializerDecorator : IJsonSerializer {
	protected IJsonSerializer InternalSerializer;

	public SerializerDecorator(IJsonSerializer internalSerializer) {
		InternalSerializer = internalSerializer;
	}

	public virtual string Serialize<T>(T value) => InternalSerializer.Serialize<T>(value);

	public virtual T Deserialize<T>(string value) => InternalSerializer.Deserialize<T>(value);
}
