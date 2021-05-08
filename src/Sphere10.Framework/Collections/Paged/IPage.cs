using System.Collections.Generic;

namespace Sphere10.Framework {
	public interface IPage<TItem> : IEnumerable<TItem> {
		int Number { get; set; }
		int StartIndex { get; set; }
		int EndIndex { get; set; }
		int Count { get; set; }
		int Size { get; set; }
		bool Dirty { get; set; }
		PageState State { get; set; }
		IEnumerable<TItem> Read(int index, int count);
		bool Write(int index, IEnumerable<TItem> items, out IEnumerable<TItem> overflow);
		void EraseFromEnd(int count);
	}
}
