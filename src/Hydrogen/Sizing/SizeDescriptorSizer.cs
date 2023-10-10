using System;

namespace Hydrogen;

/// <summary>
/// Determines the byte-size of a "size" long which can can be serialized in a variety of strategies (<see cref="SizeDescriptorStrategy"/>).
/// </summary>
public class SizeDescriptorSizer : ItemSizerBase<long> {

	public SizeDescriptorSizer() : this(SizeDescriptorStrategy.UseVarInt) {
	}

	public SizeDescriptorSizer(SizeDescriptorStrategy sizeDescriptorStrategy) {
		SizeDescriptorStrategy = sizeDescriptorStrategy;
	}

	public SizeDescriptorStrategy SizeDescriptorStrategy { get; }

	public override bool IsConstantSize => SizeDescriptorStrategy == SizeDescriptorStrategy.UseULong;

	public override long ConstantSize => IsConstantSize ? sizeof(ulong) : throw new InvalidOperationException($"Size is not statically sized for {SizeDescriptorStrategy}");

	public override long CalculateSize(SerializationContext context, long item) => SizeDescriptorStrategy switch {
		SizeDescriptorStrategy.UseVarInt => VarInt.SizeOf(unchecked((ulong)item)),
		SizeDescriptorStrategy.UseCVarInt => CVarInt.SizeOf(unchecked((ulong)item)),
		SizeDescriptorStrategy.UseULong => sizeof(ulong),
		SizeDescriptorStrategy.UseUInt32 => sizeof(uint),
		SizeDescriptorStrategy.UseUInt16 => sizeof(short),
		SizeDescriptorStrategy.UseByte => sizeof(byte),
		_ => throw new ArgumentOutOfRangeException()
	};
}
