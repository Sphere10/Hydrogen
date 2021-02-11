using System;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Digests;

namespace Sphere10.Framework {

	public class GeneralDigestAdapter : IHashFunction {
		private readonly GeneralDigest _generalDigest;
		private bool _partiallyComputed = false;
		private static readonly byte[] NullBytes = new byte[0];

		public GeneralDigestAdapter(GeneralDigest generalDigest) {
			_generalDigest = generalDigest;
			DigestSize = generalDigest.GetDigestSize();
		}

		public int DigestSize { get; }


		public void Compute(ReadOnlySpan<byte> input, Span<byte> output) {
			if (_partiallyComputed)
				throw new InvalidOperationException("Complete prior transformations before starting a new one");
			Transform(input);
			GetResult(output);
		}

		public void Transform(ReadOnlySpan<byte> part) {
			_partiallyComputed = true;
			_generalDigest.BlockUpdate(part.ToArray(), 0, part.Length);
		}

		public void GetResult(Span<byte> result) {
			if (!_partiallyComputed)
				throw new InvalidOperationException("Nothing was transformed");
			try {
				var outputArr = new byte[DigestSize];
				_generalDigest.DoFinal(outputArr, 0);
				outputArr.CopyTo(result);
			} finally {
				_generalDigest.Reset();
				_partiallyComputed = false;
			}
		}


		public void Dispose() {
		}


		public object Clone() {
			throw new NotSupportedException();
		}

	}

}
