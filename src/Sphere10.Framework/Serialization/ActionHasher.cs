using System;

namespace Sphere10.Framework {

	public class ActionHasher<T> : IItemHasher<T> {
		private readonly Func<T, byte[]> _hasher;

		public ActionHasher(Func<T, byte[]> hasher, int digestLength) {
			_hasher = hasher;
			DigestLength = digestLength;
		}

		public byte[] Hash(T @object) => _hasher.Invoke(@object);

		public int DigestLength { get; }

	}

}
