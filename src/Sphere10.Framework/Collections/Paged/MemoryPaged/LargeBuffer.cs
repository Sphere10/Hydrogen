using System;

namespace Sphere10.Framework {

    public class LargeBuffer : MemoryPagedList<byte, MemoryBufferPage> {

		public LargeBuffer(int pageSize, int inMemoryPages) 
			: base(pageSize, inMemoryPages, CacheCapacityPolicy.CapacityIsMaxOpenPages) {
		}

		protected override MemoryBufferPage NewPageInstance(int pageNumber) {
			return new MemoryBufferPage(PageSize);
		}

		protected override MemoryBufferPage[] LoadPages() {
			throw new NotSupportedException("Pages are not loadable across runtime sessions in this implementation. See BinaryFile class.");
		}

	}

}