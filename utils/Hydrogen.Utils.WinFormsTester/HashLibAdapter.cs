using System;
using System.Security.Cryptography;
using Blake2Fast;
using HashLib4CSharp.Base;
using HashLib4CSharp.Interfaces;

namespace Hydrogen {

	public class HashLibAdapter : IHashFunction {
		private readonly IHash _hashAlgorithm;
		private bool _needsFinalBlock;

		public HashLibAdapter(IHash hashAlgorithm) {
			_hashAlgorithm = hashAlgorithm;
			DigestSize = hashAlgorithm.HashSize;
		}

		public int DigestSize { get; }

		public void Compute(ReadOnlySpan<byte> input, Span<byte> output) {
			if (_needsFinalBlock)
				throw new InvalidOperationException("Complete prior transformations before starting a new one");
			_hashAlgorithm.ComputeBytes(input.ToArray()).GetBytes().CopyTo(output);
		}

		public void Transform(ReadOnlySpan<byte> part) {
			_hashAlgorithm.TransformBytes(part.ToArray());
			_needsFinalBlock = true;
			var arr = part.ToArray();
			_hashAlgorithm.TransformBytes(arr);
		}

		public void GetResult(Span<byte> result) {
			if (!_needsFinalBlock) 
				throw new InvalidOperationException("No transformations were made");
			_needsFinalBlock = false; 
			_hashAlgorithm.TransformFinal().GetBytes().CopyTo(result);
		}

		public void Dispose() {
		}

		public object Clone() {
			return _hashAlgorithm.Clone();
		}

		public static void RegisterHashLibHashers() {
			
			Hashers.Register(CHF.SHA2_256, () => new HashLibAdapter(HashFactory.Crypto.CreateSHA2_256()));
			Hashers.Register(CHF.SHA2_384, () => new HashLibAdapter(HashFactory.Crypto.CreateSHA2_384()));
			Hashers.Register(CHF.SHA2_512, () => new HashLibAdapter(HashFactory.Crypto.CreateSHA2_512()));
			Hashers.Register(CHF.SHA1_160, () => new HashLibAdapter(HashFactory.Crypto.CreateSHA1()));

			//Hashers.Register(CryptographicHashFunction.Blake2b_256, () => new HashLibAdapter(HashFactory.Crypto.CreateBlake2B_256()));
			//Hashers.Register(CryptographicHashFunction.Blake2b_160, () => new HashLibAdapter(HashFactory.Crypto.CreateBlake2B_160()));

			Hashers.Register(CHF.Blake2b_256, () => new Blake2bAdapter(32));
			Hashers.Register(CHF.Blake2b_160, () => new Blake2bAdapter(20));
			Hashers.Register(CHF.Blake2b_128, () => new Blake2bAdapter(16));

		}

		public class Blake2bAdapter : HashFunctionBase {
			IBlake2Incremental _hasher;

			public Blake2bAdapter(int digestSize) {
				DigestSize = digestSize;
				
			}

			public override int DigestSize { get; }

			public override void Initialize() {
				base.Initialize();
				_hasher = Blake2Fast.Blake2b.CreateIncrementalHasher(DigestSize);
			}

			public override void Transform(ReadOnlySpan<byte> data) {
				base.Transform(data);
				_hasher.Update(data);
				
			}

			protected override void Finalize(Span<byte> digest) {
				_hasher.Finish(digest);
			}

			public override object Clone() {
				throw new NotSupportedException();
			}
		}
	}


}
