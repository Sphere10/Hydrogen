using System;
using Org.BouncyCastle.Math.EC;

namespace Sphere10.Framework.CryptoEx.EC;

public static class Validations {
	/// <summary>
	/// Validate Private Key
	/// </summary>
	/// <param name="name"></param>
	/// <param name="buf"></param>
	/// <param name="len"></param>
	/// <param name="idx"></param>
	/// <exception cref="Exception"></exception>
	private static void ValidatePrivateKey(string name, ReadOnlySpan<byte> buf, int len, int? idx = null) {
		var idxStr = (idx.HasValue ? "[" + idx + "]" : "");
		// if (buf.IsEmpty) {
		// 	throw new Exception($"{name + idxStr} cannot be empty");
		// }
		if (buf.Length != len) {
			throw new Exception($"{name + idxStr} must be {len} bytes long");
		}
	}
	
	private static void ValidateSignatureArrays(byte[][] signatures) {
		for (var i = 0; i < signatures.Length; i++) {
			ValidateBuffer(nameof(signatures), signatures[i], 64, i);
		}
	}
	private static void ValidateMessageDigestArrays(byte[][] messageDigests) {
		for (var i = 0; i < messageDigests.Length; i++) {
			ValidateBuffer(nameof(messageDigests), messageDigests[i], 32, i);
		}
	}
	private static void ValidatePublicKeyArrays(byte[][] publicKeys) {
		for (var i = 0; i < publicKeys.Length; i++) {
			ValidateBuffer(nameof(publicKeys), publicKeys[i], 32, i);
		}
	}
	private static void ValidateNonceArrays(byte[][] nonces) {
		for (var i = 0; i < nonces.Length; i++) {
			ValidateBuffer(nameof(nonces), nonces[i], 32, i);
		}
	}
	
	/// <summary>
	/// Validate Point
	/// </summary>
	/// <param name="point"></param>
	/// <exception cref="Exception"></exception>
	internal static void ValidatePoint(ECPoint point) {
		if (point.IsInfinity) {
			throw new Exception($"{nameof(point)} is at infinity");
		}
		if (!Math.IsEven(point))
		{
			throw new Exception($"{nameof(point)} does not exist");
		}
	}
	
	/// <summary>
	/// Validate Buffers
	/// </summary>
	/// <param name="name"></param>
	/// <param name="buf"></param>
	/// <param name="len"></param>
	/// <param name="idx"></param>
	/// <exception cref="Exception"></exception>
	internal static void ValidateBuffer(string name, ReadOnlySpan<byte> buf, int len, int? idx = null) {
		var idxStr = (idx.HasValue ? "[" + idx + "]" : "");
		// if (buf.IsEmpty) {
		// 	throw new Exception($"{name + idxStr} cannot be empty");
		// }
		if (buf.Length != len) {
			throw new Exception($"{name + idxStr} must be {len} bytes long");
		}
	}

	/// <summary>
	/// Validate Signature Parameters
	/// </summary>
	/// <param name="privateKey"></param>
	/// <param name="messageDigest"></param>
	internal static void ValidateSignatureParams(byte[] privateKey, ReadOnlySpan<byte> messageDigest) {
		ValidateBuffer(nameof(messageDigest), messageDigest, 32);
		ValidatePrivateKey(nameof(privateKey), privateKey, 32);
	}
	/// <summary>
	/// Validate Verification Parameters
	/// </summary>
	/// <param name="signature"></param>
	/// <param name="messageDigest"></param>
	/// <param name="publicKey"></param>
	internal static void ValidateVerificationParams(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> publicKey) {
		ValidateBuffer(nameof(signature), signature, 64);
		ValidateBuffer(nameof(publicKey), publicKey, 32);
		ValidateBuffer(nameof(messageDigest), messageDigest, 32);
	}
	/// <summary>
	/// Validate Batch Verification Parameters
	/// </summary>
	/// <param name="signatures"></param>
	/// <param name="messageDigests"></param>
	/// <param name="publicKeys"></param>
	/// <exception cref="Exception"></exception>
	internal static void ValidateBatchVerificationParams(byte[][] signatures, byte[][] messageDigests, byte[][] publicKeys) {

		ValidateSignatureArrays(signatures);
		ValidatePublicKeyArrays(publicKeys);
		ValidateMessageDigestArrays(messageDigests);

		if (signatures.Length != messageDigests.Length || messageDigests.Length != publicKeys.Length) {
			throw new Exception("all parameters must be an array with the same length");
		}
	}
}
