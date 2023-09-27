using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class ExtendedCollectionAdapter<T> : ExtendedCollectionBase<T> {
	private readonly ICollection<T> _source;

	public ExtendedCollectionAdapter(ICollection<T> source) {
		Guard.ArgumentNotNull(source, nameof(source));
		_source = source;
	}
	
	public override long Count => _source.Count;

	public override bool IsReadOnly => _source.IsReadOnly;

	public override void Add(T item) => _source.Add(item);

	public override void AddRange(IEnumerable<T> items) => items.ForEach(Add);

	public override void Clear() => _source.Clear();

	public override bool Remove(T item) => _source.Remove(item);

	public override IEnumerable<bool> RemoveRange(IEnumerable<T> items) => items.Select(Remove);

	public override bool Contains(T item) => _source.Contains(item);

	public override IEnumerable<bool> ContainsRange(IEnumerable<T> items) => items.Select(Contains);


	public override void CopyTo(T[] array, int arrayIndex) => _source.CopyTo(array, arrayIndex);

	public override IEnumerator<T> GetEnumerator() =>_source.GetEnumerator();

}
