using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sphere10.Framework {
	public sealed class BinaryFormattedPage<TItem> : FileSwappedMemoryPage<TItem> {

		public BinaryFormattedPage(int pageSize, IObjectSizer<TItem> sizer)
			: base(pageSize, sizer, new ExtendedList<TItem>()) {
		}

		protected override void SaveInternal(IEnumerable<TItem> items, Stream stream) {
			var formatter = new BinaryFormatter();
			var itemsArr = items as TItem[] ?? items.ToArray();
			formatter.Serialize(stream, itemsArr);
		}

		protected override IEnumerable<TItem> LoadInternal(Stream stream) {
			TItem[] result;
			if (stream.Length > 0) {
				var formatter = new BinaryFormatter();
				result = (TItem[])formatter.Deserialize(stream);
			} else {
				result = new TItem[0];
			}
			Debug.Assert(result.Count() == Count);
			return result;
		}
	}
}
