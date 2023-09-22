namespace Hydrogen;

public class ConstantLengthArraySerializer<T> : ItemSerializer<T[]> {
	private readonly IAutoSizedSerializer<T> _valueSerializer;
	private readonly long _arrayLength;
	
	public ConstantLengthArraySerializer(IAutoSizedSerializer<T> valueSerializer, SizeDescriptorStrategy valueSizeDescriptorStrategy,  long arrayLength) {
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		Guard.ArgumentInRange(arrayLength, 0, long.MaxValue, nameof(arrayLength));
		_arrayLength = arrayLength;
		
		if (valueSerializer is not IAutoSizedSerializer<T> autoSizedSerializer) {
			if (valueSerializer.IsConstantLength)
				autoSizedSerializer = new ConstantLengthSerializer<T>(valueSerializer);
			else
				autoSizedSerializer = new AutoSizedSerializer<T>(valueSerializer, valueSizeDescriptorStrategy);
		}
		_valueSerializer = autoSizedSerializer;	
	}

	public override long CalculateSize(T[] item) 
		=> _valueSerializer.CalculateTotalSize(item, false, out _);

	public override void SerializeInternal(T[] item, EndianBinaryWriter writer) {
		Guard.Ensure(item.Length == _arrayLength, $"Array length must be {_arrayLength}");
		foreach(var element in item)
			_valueSerializer.SerializeInternal(element, writer);
	}

	public override T[] DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		var arraySize = ConstantLength;
		var array = new T[arraySize];
		for (var i = 0; i < arraySize; i++) {
			array[i] = _valueSerializer.Deserialize(reader);
		}
		return array;
	}
}
