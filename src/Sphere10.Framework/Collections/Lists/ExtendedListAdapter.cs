using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// Converts an IList to IExtendedList. 
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class ExtendedListAdapter<TItem> : SingularListBase<TItem> {

		private readonly IList<TItem> _endpoint;

		public ExtendedListAdapter() 
			: this( new List<TItem>()) { 
		}

		public ExtendedListAdapter(IList<TItem> endpoint) {
			_endpoint = endpoint;
		}

		public override int Count => _endpoint.Count;

		public override bool IsReadOnly => _endpoint.IsReadOnly;

		public override int IndexOf(TItem item) => _endpoint.IndexOf(item);

		public override bool Contains(TItem item) {
			return _endpoint.Contains(item);
		}

		public override IEnumerable<bool> ContainsRange(IEnumerable<TItem> items) => items.Select(Contains).ToArray();

		public override TItem Read(int index) => _endpoint[index];

		public override void Add(TItem item) => _endpoint.Add(item);

		public override void Update(int index, TItem item) => _endpoint[index] = item;

		public override void Insert(int index, TItem item) => _endpoint.Insert(index, item);

		public override void RemoveAt(int index) => _endpoint.RemoveAt(index);

		public override IEnumerable<bool> RemoveRange(IEnumerable<TItem> items) => items.Select(Remove).ToArray();

		public override void Clear() => _endpoint.Clear();

		public override bool Remove(TItem item) {
			return _endpoint.Remove(item);
		}

		public override void CopyTo(TItem[] array, int arrayIndex) {
			_endpoint.CopyTo(array, arrayIndex);
		}

		public override IEnumerator<TItem> GetEnumerator() {
			return _endpoint.GetEnumerator();
		}

	}
}