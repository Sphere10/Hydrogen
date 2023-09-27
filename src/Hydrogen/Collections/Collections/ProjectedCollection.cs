using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public sealed class ProjectedCollection<T, TProjection> : ICollection<TProjection> {
	private readonly ICollection<T> _source;
	private readonly Func<T, TProjection> _projection;
	private readonly Func<TProjection, T> _inverseProjection;

	public ProjectedCollection(ICollection<T> source, Func<T, TProjection> projection, Func<TProjection, T> inverseProjection) {
		Guard.ArgumentNotNull(source, nameof(source));
		Guard.ArgumentNotNull(projection, nameof(projection));
		Guard.ArgumentNotNull(inverseProjection, nameof(inverseProjection));
		_source = source;
		_projection = projection;
		_inverseProjection = inverseProjection;
	}
	
	public int Count => _source.Count;
	
	public bool IsReadOnly => _source.IsReadOnly;

	public void Add(TProjection item) => _source.Add(_inverseProjection(item));

	public void Clear() => _source.Clear();

	public bool Contains(TProjection item) => _source.Contains(_inverseProjection(item));

	public void CopyTo(TProjection[] array, int arrayIndex) => _source.Select(_projection).ToArray().CopyTo(array, arrayIndex);

	public bool Remove(TProjection item) => _source.Remove(_inverseProjection(item));

	public IEnumerator<TProjection> GetEnumerator() {
		foreach (var item in _source) {
			yield return _projection(item);
		}
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

}