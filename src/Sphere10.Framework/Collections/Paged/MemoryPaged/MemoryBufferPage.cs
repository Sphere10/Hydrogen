using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	public class MemoryBufferPage : FileSwappedMemoryPage<byte>, IBinaryPage {

		public MemoryBufferPage(int pageSize)
			: base(pageSize, new ConstantObjectSizer<byte>(sizeof(byte)), new MemoryBuffer(0, pageSize, pageSize)) {
		}

		public ReadOnlySpan<byte> ReadSpan(int index, int count) {
			CheckPageState(PageState.Loaded);
			CheckRange(index, count);
			return ((MemoryBuffer)base.MemoryStore).ReadSpan(index - StartIndex, count);
		}

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