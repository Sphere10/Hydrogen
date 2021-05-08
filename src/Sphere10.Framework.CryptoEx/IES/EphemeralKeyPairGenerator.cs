using Org.BouncyCastle.Crypto;

namespace Sphere10.Framework.CryptoEx.IES {
	public class EphemeralKeyPairGenerator
    {
        private readonly IAsymmetricCipherKeyPairGenerator _gen;
        private readonly KeyEncoder _keyEncoder;

        public EphemeralKeyPairGenerator(IAsymmetricCipherKeyPairGenerator gen, KeyEncoder keyEncoder)
        {
            _gen = gen;
            _keyEncoder = keyEncoder;
        }

        public EphemeralKeyPair Generate()
        {
            AsymmetricCipherKeyPair eph = _gen.GenerateKeyPair();

            // Encode the ephemeral public key
            return new EphemeralKeyPair(eph, _keyEncoder);
        }
    }
}