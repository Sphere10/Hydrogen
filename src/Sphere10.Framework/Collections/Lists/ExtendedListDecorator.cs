using System.Collections;
using System.Collections.Generic;

namespace Sphere10.Framework {

	/// <summary>
	/// Decorator pattern for an IExtendedList, but calls to non-range get routed to the range-based methods.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <remarks>At first glance the implementation may counter-intuitive, but it is born out of extensive usage and optimization. The <see cref="TConcrete"/>
	/// generic argument ensures sub-classes can retrieve the decorated list in it's type,without an expensive chain of casts/retrieves.</remarks>
	public abstract class ExtendedListDecorator<TItem> : ExtendedListDecorator<TItem, IExtendedList<TItem>> {
		protected ExtendedListDecorator(IExtendedList<TItem> internalExtendedList)
			: base(internalExtendedList) {
		}
	}

	/// <summary>
	/// Decorator pattern for an IExtendedList, but calls to non-range get routed to the range-based methods.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <typeparam name="TConcrete"></typeparam>
	/// <remarks>At first glance the implementation may counter-intuitive, but it is born out of extensive usage and optimization. The <see cref="TConcrete"/>
	/// generic argument ensures sub-classes can retrieve the decorated list in it's type,without an expensive chain of casts/retrieves.</remarks>
	public abstract class ExtendedListDecorator<TItem, TConcrete> : CollectionDecorator<TItem, TConcrete>, IExtendedList<TItem>
		where TConcrete : IExtendedList<TItem> {
		//protected TConcrete InternalCollection;

		protected ExtendedListDecorator(TConcrete internalExtendedList) : base(internalExtendedList) {
		}

		public virtual int IndexOf(TItem item) => InternalCollection.IndexOf(item);

		public virtual IEnumerable<int> IndexOfRange(IEnumerable<TItem> items) => InternalCollection.IndexOfRange(items);

		public virtual IEnumerable<bool> ContainsRange(IEnumerable<TItem> items) => InternalCollection.ContainsRange(items);

		public virtual TItem Read(int index) => InternalCollection.Read(index);

		public virtual IEnumerable<TItem> ReadRange(int index, int count) => InternalCollection.ReadRange(index, count);

		public virtual void AddRange(IEnumerable<TItem> items) => InternalCollection.AddRange(items);

		public virtual void Update(int index, TItem item) => InternalCollection.Update(index, item);

		public virtual void UpdateRange(int index, IEnumerable<TItem> items) => InternalCollection.UpdateRange(index, items);

		public virtual void Insert(int index, TItem item) => InternalCollection.Insert(index, item);

		public virtual void InsertRange(int index, IEnumerable<TItem> items) => InternalCollection.InsertRange(index, items);

		public virtual IEnumerable<bool> RemoveRange(IEnumerable<TItem> items) => InternalCollection.RemoveRange(items);

		public virtual void RemoveAt(int index) => InternalCollection.RemoveAt(index);

		public virtual void RemoveRange(int index, int count) => InternalCollection.RemoveRange(index, count);

		public TItem this[int index] { get => Read(index); set => this.Update(index, value); }

		TItem IWriteOnlyExtendedList<TItem>.this[int index] { set => this[index] = value; }

		TItem IReadOnlyList<TItem>.this[int index] => this[index];

	}

}
