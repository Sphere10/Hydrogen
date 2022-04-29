using System;

namespace Sphere10.Framework {

	public interface IIESAlgorithm {
		byte[] Encrypt(ReadOnlySpan<byte> message, IPublicKey publicKey);
		bool TryDecrypt(ReadOnlySpan<byte> encryptedMessage, out byte[] decryptedMessage, IPrivateKey privateKey);
	}


	public static class IIESAlgorithmExtensions {
		public static byte[] Decrypt(this IIESAlgorithm iesAlgorithm, ReadOnlySpan<byte> encryptedMessage, IPrivateKey privateKey) {
			if (!iesAlgorithm.TryDecrypt(encryptedMessage, out var decryptedMessage, privateKey))
				throw new InvalidOperationException("Unable to decrypt message");   // TODO: add proper exception types
			return decryptedMessage;
		}
	}
	
}