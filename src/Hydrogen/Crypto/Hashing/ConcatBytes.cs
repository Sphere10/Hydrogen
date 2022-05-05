using System;

namespace Hydrogen {

	/// <summary>
	/// A mock hash algorithm used for primarily for testing merkle-trees.
	/// </summary>
	internal class ConcatBytes  : HashFunctionBase {
		private readonly ByteArrayBuilder _builder;

		public ConcatBytes() {
			_builder = new ByteArrayBuilder();
		}

		public override int DigestSize => _builder.Length;

		public override void Transform(ReadOnlySpan<byte> data) {
			base.Transform(data);
			_builder.Append(data.ToArray());
		}

		protected override void Finalize(Span<byte> digest) {
			_builder.ToArray().AsSpan().CopyTo(digest);
			_builder.Clear();
		}

		public override object Clone() {
			throw new NotImplementedException();
		}
	}

}

