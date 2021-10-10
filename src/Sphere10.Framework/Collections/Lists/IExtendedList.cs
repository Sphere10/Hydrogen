using System.Collections.Generic;

namespace Sphere10.Framework {

	public interface IExtendedList<T> : IExtendedCollection<T>, IList<T>, IReadOnlyExtendedList<T>, IWriteOnlyExtendedList<T> {
		new T this[int index] { get; set; }
		new int IndexOf(T item);
		new void Insert(int index, T item);
		new void RemoveAt(int index);


	}

	public static class IExtendedListExtensions {

		public static IExtendedList<TTo> CastListAs<TFrom, TTo>(this IExtendedList<TFrom> list) where TFrom : class where TTo : class {
			if (list is CastedExtendedList<TTo, TFrom> casted) // if casting back, use original
				return casted._from;
			return new CastedExtendedList<TFrom, TTo>(list);
		}
	}

}