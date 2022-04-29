using System;

namespace Sphere10.Framework {

	public interface IOTSAlgorithm {
		OTSConfig Config { get; }
		void SerializeParameters(Span<byte> buffer);
		void ComputeKeyHash(byte[,] key, Span<byte> result);
		byte[,] SignDigest(byte[,] privateKey, ReadOnlySpan<byte> digest);
		bool VerifyDigest(byte[,] signature, byte[,] publicKey, ReadOnlySpan<byte> digest);
		OTSKeyPair GenerateKeys(ReadOnlySpan<byte> seed);
	}

	public static class IOTSAlgorithmExtensions {
		public static byte[] ComputeKeyHash(this IOTSAlgorithm algo, byte[,] key) {
			var result = new byte[algo.Config.DigestSize];
			algo.ComputeKeyHash(key, result);
			return result;
		}
	}
}
