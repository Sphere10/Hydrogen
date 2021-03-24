using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Sphere10.Framework {

	public class ObjectHasher<TItem> : ObjectSerializerDecorator<TItem>, IObjectHasher<TItem> {
		private readonly CHF _hashAlgorithm;

		public ObjectHasher(IObjectSerializer<TItem> internalSerializer)
			: this(CHF.SHA2_256, internalSerializer) {
		}

		public ObjectHasher(CHF hashAlgorithm, IObjectSerializer<TItem> internalSerializer)
			: base(internalSerializer) {
			_hashAlgorithm = hashAlgorithm;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte[] Hash(TItem @object) => Hashers.Hash(_hashAlgorithm, @object, this);
	}

}