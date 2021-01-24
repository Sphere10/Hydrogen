using System.Collections.Generic;

namespace Sphere10.Framework {
    public interface IMemoryPagedBuffer : IMemoryPagedList<byte>, IBuffer {
        new IReadOnlyList<IBufferPage> Pages { get; }
    }
}
