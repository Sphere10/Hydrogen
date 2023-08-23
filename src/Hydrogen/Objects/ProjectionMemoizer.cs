using System;
using Hydrogen;


public class ProjectionMemoizer {

	private readonly SynchronizedDictionary<object, object> _projections = new();

	public V RememberProjection<U, V>(U item, Func<U, V> packer) {
		using (_projections.EnterWriteScope()) {
			if (_projections.TryGetValue(item, out _)) {
				throw new InvalidOperationException("Item already packed");
			}
			var packedItem = packer(item);
			_projections.Add(item, packedItem);
			return packedItem;
		}
	}

	public V ForgetProjection<U, V>(U item) {
		using (_projections.EnterWriteScope()) {
			if (!_projections.TryGetValue(item, out var packedItem)) {
				throw new InvalidOperationException("Item was not packed");
			}
			_projections.Remove(item);
			var packedItemT = (V)(object)packedItem;
			return packedItemT;
		}
	}
}
