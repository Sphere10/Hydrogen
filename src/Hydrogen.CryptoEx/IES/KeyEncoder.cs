using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace Hydrogen.CryptoEx.IES {
	public class KeyEncoder
    {
        private readonly bool _usePointCompression;

        public KeyEncoder(bool usePointCompression)
        {
            _usePointCompression = usePointCompression;
        }

        public byte[] GetEncoded(AsymmetricKeyParameter keyParameter)
        {
            return (keyParameter as ECPublicKeyParameters)?.Q.GetEncoded(_usePointCompression);
        }
    }
}