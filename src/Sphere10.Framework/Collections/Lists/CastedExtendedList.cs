using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	public sealed class CastedExtendedList<TFrom, TTo> : IExtendedList<TTo> 
		where TFrom : class 
		where TTo : class  {
		internal readonly IExtendedList<TFrom> _from;

		public CastedExtendedList(IExtendedList<TFrom> from)  {
			_from = from;
		}

		public int Count => _from.Count;

		public bool IsReadOnly => _from.IsReadOnly;

		public int IndexOf(TTo item) => _from.IndexOf(item as TFrom);

		public IEnumerable<int> IndexOfRange(IEnumerable<TTo> items) => _from.IndexOfRange(items.Cast<TFrom>());

		public bool Contains(TTo item) => _from.Contains(item as TFrom);

		public IEnumerable<bool> ContainsRange(IEnumerable<TTo> items) => _from.ContainsRange(items.Cast<TFrom>());

		public TTo Read(int index) => _from.Read(index) as TTo;

		public IEnumerable<TTo> ReadRange(int index, int count) => _from.ReadRange(index, count).Cast<TTo>();

		public void Add(TTo item) => _from.Add(item as TFrom);

		public void AddRange(IEnumerable<TTo> items) => _from.AddRange(items.Cast<TFrom>());

		public void Update(int index, TTo item) => _from.Update(index, item as TFrom);

		public void UpdateRange(int index, IEnumerable<TTo> items) => _from.UpdateRange(index, items.Cast<TFrom>());

		public void Insert(int index, TTo item) => _from.Insert(index, item as TFrom);

		public void InsertRange(int index, IEnumerable<TTo> items) => _from.InsertRange(index, items.Cast<TFrom>());

		public bool Remove(TTo item) => _from.Remove(item as TFrom);

		public IEnumerable<bool> RemoveRange(IEnumerable<TTo> items) => _from.RemoveRange(items.Cast<TFrom>());

		public void RemoveAt(int index) => _from.RemoveAt(index);

		public void RemoveRange(int index, int count) => _from.RemoveRange(index, count);

		public void Clear() => _from.Clear();

		public void CopyTo(TTo[] array, int arrayIndex) => _from.CopyTo(System.Array.ConvertAll(array, x => x as TFrom), arrayIndex);

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public IEnumerator<TTo> GetEnumerator() => _from.Cast<TTo>().GetEnumerator();

		public TTo this[int index] { get => this.Read(index); set => this.Update(index, value); }

		TTo IWriteOnlyExtendedList<TTo>.this[int index] { set => this[index] = value; }

		TTo IReadOnlyList<TTo>.this[int index] => this[index];
	}

}
