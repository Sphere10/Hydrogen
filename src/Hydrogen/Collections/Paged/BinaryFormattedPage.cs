using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sphere10.Framework {
	public sealed class BinaryFormattedPage<TItem> : FileSwappedMemoryPage<TItem> {

		public BinaryFormattedPage(int pageSize, IItemSizer<TItem> sizer)
			: base(pageSize, sizer, new ExtendedList<TItem>()) {
		}

		protected override void SaveInternal(IExtendedList<TItem> memoryPage, Stream stream) {
			var formatter = new BinaryFormatter();
			var itemsArr = memoryPage as TItem[] ?? memoryPage.ToArray();
			formatter.Serialize(stream, itemsArr);
			stream.SetLength(Math.Max(0, stream.Position)); // end stream after serialization
		}

		protected override void LoadInternal(Stream stream, IExtendedList<TItem> memoryPage) {
			TItem[] result;
			if (stream.Length > 0) {
				var formatter = new BinaryFormatter();
				result = (TItem[])formatter.Deserialize(stream);
			} else {
				result = new TItem[0];
			}
			memoryPage.AddRange(result);
			Debug.Assert(result.Count() == Count);
		}
	}
}
