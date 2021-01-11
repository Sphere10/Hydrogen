using System;

namespace Sphere10.Framework {
    public interface IBinaryPage : IPage<byte> {
		ReadOnlySpan<byte> ReadSpan(int index, int count);
	}
}
