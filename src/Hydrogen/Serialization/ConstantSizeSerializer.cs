namespace Hydrogen;

internal class ConstantSizeSerializer<T> : ItemSerializerDecorator<T> {
	public ConstantSizeSerializer(IItemSerializer<T> internalSerializer) 
		: base(internalSerializer) {
		Guard.Argument(internalSerializer.IsConstantSize, nameof(internalSerializer), "Serializer must be constant length");
	}

}
