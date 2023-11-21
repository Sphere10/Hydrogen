using System;

namespace Hydrogen;


public sealed class ConstantSizeReferenceTypeSerializer<TItem> : ItemSerializerDecorator<TItem>  {
	private readonly long _internalConstantSize;
	private readonly byte[] _padding;

	public ConstantSizeReferenceTypeSerializer(ReferenceSerializer<TItem> referenceSerializer) 
		: base(referenceSerializer) {
		Guard.Argument(referenceSerializer.Internal.IsConstantSize, nameof(referenceSerializer), $"{nameof(ReferenceSerializer<TItem>)}'s internal serializer must be a constant size serializer");
		_internalConstantSize = referenceSerializer.Internal.ConstantSize;
		_padding = new byte[_internalConstantSize];
	}

	public override long ConstantSize => sizeof(byte) + _internalConstantSize;

	public override long CalculateSize(SerializationContext context, TItem item) => sizeof(bool) + _internalConstantSize;

	public override void Serialize(TItem item, EndianBinaryWriter writer, SerializationContext context) {
		base.Serialize(item, writer, context);
		if (item is null)
			writer.Write(_padding);
	}

	public override TItem Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var item = base.Deserialize(reader, context);
		if (item is null)
			reader.ReadBytes(_internalConstantSize);
		return item;
	}

}
