using System.Collections.Generic;

namespace Sphere10.Framework {

	public class IndexItemsCallArgs<T> : CallArgs {
		public IndexItemsCallArgs(int index, T item) : base(index, new[] { item }) {
		}

		public IndexItemsCallArgs(int index, IEnumerable<T> items) : base(index, items) {
		}

		public int Index { get => (int)base[0]; set => base[0] = value; }
		public IEnumerable<T> Items { get => (IEnumerable<T>)base[1]; set => base[1] = value; }
	}

}