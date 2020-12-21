using System;
using System.Security.Cryptography;

namespace Sphere10.Framework {

	public class HashAlgorithmAdapter : IHashFunction {
		private readonly HashAlgorithm _hashAlgorithm;
		private bool _needsFinalBlock = false;
		private static readonly byte[] NullBytes = new byte[0];

		public HashAlgorithmAdapter(HashAlgorithm hashAlgorithm) {
			_hashAlgorithm = hashAlgorithm;
			DigestSize = hashAlgorithm.HashSize >> 3;
		}

		public int DigestSize { get; }


		public void Compute(ReadOnlySpan<byte> input, Span<byte> output) {
			if (_needsFinalBlock)
				throw new InvalidOperationException("Complete prior transformations before starting a new one");

			if (!_hashAlgorithm.TryComputeHash(input, output, out _))
				throw new InvalidOperationException();
		}

		public void Transform(ReadOnlySpan<byte> part) {
			// Slow
			_needsFinalBlock = true;
			var arr = part.ToArray();
			_hashAlgorithm.TransformBlock(arr, 0, arr.Length, null, 0);
		}

		public void GetResult(Span<byte> result) {
			if (_needsFinalBlock) {
				_hashAlgorithm.TransformFinalBlock(NullBytes, 0, 0);
				_needsFinalBlock = false;
			}
			_hashAlgorithm.Hash.CopyTo(result);
		}


		public void Dispose() {
			_hashAlgorithm.Dispose();
		}


		public object Clone() {
			throw new NotSupportedException();
		}

	}

}
