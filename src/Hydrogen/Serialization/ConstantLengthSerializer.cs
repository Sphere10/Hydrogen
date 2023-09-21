namespace Hydrogen;

internal class ConstantLengthSerializer<T> : ItemSerializerDecorator<T>, IAutoSizedSerializer<T> {
	public ConstantLengthSerializer(IItemSerializer<T> internalSerializer) 
		: base(internalSerializer) {
		Guard.Argument(internalSerializer.IsConstantLength, nameof(internalSerializer), "Serializer must be constant length");
	}

	public T Deserialize(EndianBinaryReader reader) 
		=> ((IAutoSizedSerializer<T>)Internal).Deserialize(reader);

}
