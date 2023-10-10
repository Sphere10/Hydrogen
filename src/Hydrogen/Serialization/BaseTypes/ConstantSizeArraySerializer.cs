namespace Hydrogen;

public class ConstantSizeArraySerializer<T> : ConstantSizeItemSerializerBase<T[]> {
	private readonly IItemSerializer<T> _valueSerializer;
	private readonly long _arrayLength;
	
	public ConstantSizeArraySerializer(IItemSerializer<T> valueSerializer, long arrayLength) : base(arrayLength * valueSerializer.ConstantSize, false) {
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		Guard.ArgumentInRange(arrayLength, 0, long.MaxValue, nameof(arrayLength));
		Guard.Ensure(valueSerializer.IsConstantSize, "Value serializer must be constant size");
		_arrayLength = arrayLength;
		_valueSerializer = valueSerializer;	
	}

	public override void Serialize(T[] item, EndianBinaryWriter writer, SerializationContext context) {
		Guard.Ensure(item.Length == _arrayLength, $"Array length must be {_arrayLength}");
		foreach(var element in item)
			_valueSerializer.Serialize(element, writer, context);
	}

	public override T[] Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var arraySize = ConstantSize;
		var array = new T[arraySize];
		for (var i = 0; i < arraySize; i++) 
			array[i] = _valueSerializer.Deserialize(reader, context);
		return array;
	}
}
