using System.Collections.Generic;

namespace Sphere10.Framework {

	public class ItemsCallArgs<T> : CallArgs {

		public ItemsCallArgs(T item) : this(new []{item}) {
		}


		public ItemsCallArgs(IEnumerable<T> items) : base(items) {
		}

		public IEnumerable<T> Items { get => (IEnumerable<T>)base[0]; set => base[0] = value; }
	}

}