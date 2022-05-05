using System;

namespace Hydrogen {

	public class ActionHasher<T> : IItemHasher<T> {
		private readonly Func<T, byte[]> _hasher;

		public ActionHasher(Func<T, byte[]> hasher, int digestLength) {
			_hasher = hasher;
			DigestLength = digestLength;
		}

		public byte[] Hash(T item) => _hasher.Invoke(item);

		public int DigestLength { get; }

	}

}
