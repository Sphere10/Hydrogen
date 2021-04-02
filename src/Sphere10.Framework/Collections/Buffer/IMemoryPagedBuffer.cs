using System.Collections.Generic;

namespace Sphere10.Framework {
    public interface IMemoryPagedBuffer : IMemoryPagedList<byte>, IBuffer {
        internal new IReadOnlyList<IBufferPage> Pages { get; }
    }
}
