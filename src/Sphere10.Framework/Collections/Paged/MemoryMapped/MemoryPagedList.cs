using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading;

namespace Sphere10.Framework {

	public class MemoryPagedList<TItem> : MemoryPagedListBase<TItem, MemoryPagedList<TItem>.BinaryFormattedPage> {
		private readonly IObjectSizer<TItem> _sizer;

		public MemoryPagedList(int pageSize, int maxOpenPages, int fixedItemSize)
			: this(pageSize, maxOpenPages, new ConstantObjectSizer<TItem>(fixedItemSize)) {
		}

		public MemoryPagedList(int pageSize, int maxOpenPages, Func<TItem, int> itemSizer)
			: this(pageSize,  maxOpenPages, new ActionObjectSizer<TItem>(itemSizer)) {
		}

		private MemoryPagedList(int pageSize, int maxOpenPages, IObjectSizer<TItem> sizer)
			: base(pageSize, maxOpenPages, CacheCapacityPolicy.CapacityIsMaxOpenPages) {
			_sizer = sizer;
		}

		protected override BinaryFormattedPage NewPageInstance(int pageNumber) {
			return new BinaryFormattedPage(this.PageSize, _sizer);
		}

		public sealed class BinaryFormattedPage : FileSwappedMemoryPage<TItem> {

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
}