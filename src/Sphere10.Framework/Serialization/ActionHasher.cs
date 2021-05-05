using System;

namespace Sphere10.Framework {

	public class ActionHasher<T> : IItemHasher<T> {
		private readonly Func<T, byte[]> _hasher;

		public ActionHasher(Func<T, byte[]> hasher) {
			_hasher = hasher;
		}

		public byte[] Hash(T @object) => _hasher.Invoke(@object);

	}

}
