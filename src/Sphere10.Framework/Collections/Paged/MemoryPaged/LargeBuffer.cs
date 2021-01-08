using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	public class LargeBuffer : MemoryPagedList<byte, LargeBuffer.BufferPage> {

		public LargeBuffer(int pageSize, int inMemoryPages) 
			: base(pageSize, inMemoryPages, CacheCapacityPolicy.CapacityIsMaxOpenPages) {
		}

		protected override BufferPage NewPageInstance(int pageNumber) {
			return new BufferPage(PageSize);
		}

		protected override BufferPage[] LoadPages() {
			throw new NotSupportedException("Pages are not loadable across runtime sessions in this implementation. See BinaryFile class.");
		}


		public class BufferPage : FileSwappedMemoryPage<byte> {
			
			public BufferPage(int pageSize) 
				: base(pageSize, new ConstantObjectSizer<byte>(sizeof(byte)), new MemoryBuffer(0, pageSize, pageSize)) {
			}

			protected new MemoryBuffer MemoryStore => (MemoryBuffer)base.MemoryStore;

			protected override void SaveInternal(IEnumerable<byte> items, Stream stream) {
				// Use byte streaming for perf
				using (var writer = new BinaryWriter(stream)) {
					var itemsArr = items as byte[] ?? items.ToArray();
					writer.Write(itemsArr);
				}
			}

			protected override IEnumerable<byte> LoadInternal(Stream stream) {
				// Use byte streaming for perf
				var buff = new byte[stream.Length];
				var bytesRead = stream.Read(buff, 0, (int)stream.Length);
				if (bytesRead != buff.Length)
					Array.Resize(ref buff, bytesRead);
				return buff;
			}
		}

	}

}