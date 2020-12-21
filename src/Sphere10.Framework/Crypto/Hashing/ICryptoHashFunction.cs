using System;
using System.Runtime.CompilerServices;

namespace Sphere10.Framework {

	public interface IHashFunction : ICloneable, IDisposable {

		int DigestSize { get; }

		void Compute(ReadOnlySpan<byte> input, Span<byte> output);

		void Transform(ReadOnlySpan<byte> data);

		void GetResult(Span<byte> result);
	}

	public static class ICryptoHashFunctionExtensions {
	
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] GetResult(this IHashFunction chf) {
			var result = new byte[chf.DigestSize];
			chf.GetResult(result);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] Compute(this IHashFunction chf, ReadOnlySpan<byte> input) {
			chf.Transform(input);
			return chf.GetResult();
		}
	}

}
