using System;

namespace Sphere10.Framework.CryptoEx.IES
{

    public interface IIntegratedEncryptionStandard
    {
        byte[] Encrypt(ReadOnlySpan<byte> message, IPublicKey publicKey);

        bool TryDecrypt(ReadOnlySpan<byte> encryptedMessage, out ReadOnlySpan<byte> decryptedMessage,
            IPrivateKey privateKey);
    }

}