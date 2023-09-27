using System;

namespace Hydrogen;

public enum SizeDescriptorStrategy {

	UseVarInt,
	UseCVarInt,
	UseULong,
	UseUInt32,
	UseUInt16,
	UseByte
}


public static class SizeDescriptorStrategyExtensions {

	public static bool IsConstantSize(this SizeDescriptorStrategy sizeDescriptorStrategy) {
		return sizeDescriptorStrategy switch {
			SizeDescriptorStrategy.UseVarInt => false,
			SizeDescriptorStrategy.UseCVarInt => false,
			SizeDescriptorStrategy.UseULong => true,
			SizeDescriptorStrategy.UseUInt32 => true,
			SizeDescriptorStrategy.UseUInt16 => true,
			SizeDescriptorStrategy.UseByte => true,
			_ => throw new ArgumentOutOfRangeException(nameof(sizeDescriptorStrategy), sizeDescriptorStrategy, null)
		};
	}
}
