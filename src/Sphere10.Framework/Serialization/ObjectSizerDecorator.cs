using System.Collections.Generic;

namespace Sphere10.Framework {

	public class ObjectSizerDecorator<T> : IObjectSizer<T> {

		public ObjectSizerDecorator(IObjectSizer<T> internalSizer) {
			Internal = internalSizer;
		}

		protected IObjectSizer<T> Internal { get; }

		public virtual bool IsFixedSize => Internal.IsFixedSize;

		public virtual int FixedSize => Internal.FixedSize;

		public virtual int CalculateTotalSize(IEnumerable<T> items, bool calculateIndividualItems, out int[] itemSizes) => Internal.CalculateTotalSize(items, calculateIndividualItems, out itemSizes);

		public virtual int CalculateSize(T item) => Internal.CalculateSize(item);
	}

}