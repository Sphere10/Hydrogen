using System.Diagnostics;

namespace Hydrogen;

public class PaddedSerializer<TItem> : StaticSizeItemSerializerBase<TItem> {
	private readonly IItemSerializer<TItem> _dynamicSerializer;
	public PaddedSerializer(int fixedSize, IItemSerializer<TItem> dynamicSerializer) 
		: base(fixedSize) {
		_dynamicSerializer = dynamicSerializer;
	}

	public override bool TrySerialize(TItem item, EndianBinaryWriter writer) {
		var bytesWritten = 0;
		var expectedSize = _dynamicSerializer.CalculateSize(item);
		if (expectedSize > StaticSize - sizeof(int)) {
			// Item is too large to fit in StaticSize bytes
			return false;
		}

		writer.Write(expectedSize);
		bytesWritten += sizeof(int);
		if (!_dynamicSerializer.TrySerialize(item, writer, out var itemBytes))
			return false;

		if (itemBytes != expectedSize)
			return false;

		bytesWritten += itemBytes;
		if (bytesWritten > StaticSize)
			return false;

		var remaining  = StaticSize - bytesWritten;
		// TODO: should chunk this out
		var padding = Tools.Array.Gen<byte>(remaining, 0);
		writer.Write(padding);
		bytesWritten += remaining;
		Debug.Assert(bytesWritten == StaticSize);
		return true;
	}

	public override bool TryDeserialize( EndianBinaryReader reader, out TItem item) {
		var itemSize = reader.ReadInt32();
		if (itemSize > StaticSize - sizeof(int)) {
			item = default;
			return false;
		}

		if (!_dynamicSerializer.TryDeserialize(itemSize, reader, out item))
			return false;

		var padding = StaticSize - itemSize - sizeof(int);
		var _ = reader.ReadBytes(padding);
		return true;
	}
}
