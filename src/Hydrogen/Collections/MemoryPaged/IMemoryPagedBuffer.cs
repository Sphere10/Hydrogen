using System.Collections.Generic;

namespace Hydrogen {
	public interface IMemoryPagedBuffer : IMemoryPagedList<byte>, IBuffer {
        new IReadOnlyList<IBufferPage> Pages { get; }
    }
}
