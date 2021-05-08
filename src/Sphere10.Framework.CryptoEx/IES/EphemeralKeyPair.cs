using Org.BouncyCastle.Crypto;

namespace Sphere10.Framework.CryptoEx.IES {
	public class EphemeralKeyPair
    {
        private readonly AsymmetricCipherKeyPair _keyPair;
        private readonly KeyEncoder _publicKeyEncoder;

        public EphemeralKeyPair(AsymmetricCipherKeyPair keyPair, KeyEncoder publicKeyEncoder)
        {
            _keyPair = keyPair;
            _publicKeyEncoder = publicKeyEncoder;
        }

        public AsymmetricCipherKeyPair GetKeyPair()
        {
            return _keyPair;
        }

        public byte[] GetEncodedPublicKey()
        {
            return _publicKeyEncoder.GetEncoded(_keyPair.Public);
        }
    }
}