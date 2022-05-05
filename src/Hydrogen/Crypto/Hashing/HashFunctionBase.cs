using System;

namespace Hydrogen {

	public abstract class HashFunctionBase : IHashFunction {
		protected bool _inTransform;
		private byte[] _digest;
	
		public abstract int DigestSize { get; }

		public virtual void Initialize() {
			_digest ??= new byte[DigestSize];
		}

		public virtual void Compute(ReadOnlySpan<byte> input, Span<byte> output) {
			// Standard single transformation, sub-class can override for performance
			Transform(input);
			GetResult(output);
		}

		public virtual void Transform(ReadOnlySpan<byte> data) {
			if (!_inTransform) {
				Initialize();
				_inTransform = true;
			}
			// Base-class implementation will perform the hashing
		}

		protected abstract void Finalize(Span<byte> digest);

		public void GetResult(Span<byte> result) {
			if (_inTransform) {
				 if (_digest.Length != DigestSize)
					Array.Resize(ref _digest, DigestSize);
				Finalize(_digest);
				_inTransform = false;
			}
			_digest.CopyTo(result);
		}

		public abstract object Clone();

		public virtual void Dispose() {
		}


	}

}
