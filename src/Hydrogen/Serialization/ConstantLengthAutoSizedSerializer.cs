namespace Hydrogen;

internal class ConstantLengthAutoSizedSerializer<T> : ItemSerializerDecorator<T>, IAutoSizedSerializer<T> {
	public ConstantLengthAutoSizedSerializer(IItemSerializer<T> internalSerializer) 
		: base(internalSerializer) {
		Guard.Argument(internalSerializer.IsConstantLength, nameof(internalSerializer), "Serializer must be constant length");
	}

	public T Deserialize(EndianBinaryReader reader) 
		=> ((IAutoSizedSerializer<T>)Internal).Deserialize(reader);

}
