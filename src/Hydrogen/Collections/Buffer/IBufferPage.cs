using System;

namespace Sphere10.Framework {
	public interface IBufferPage : IMemoryPage<byte> {
        ReadOnlySpan<byte> ReadSpan(int index, int count);
        
		bool AppendSpan(ReadOnlySpan<byte> items, out ReadOnlySpan<byte> overflow);

        void UpdateSpan(int index, ReadOnlySpan<byte> items);
    }
}
