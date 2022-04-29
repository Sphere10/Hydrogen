using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

namespace Sphere10.Framework.CryptoEx.PascalCoin {
	public class PascalCoinEciesKdfBytesGenerator : BaseKdfBytesGenerator
    {
        private byte[] _shared;

        public PascalCoinEciesKdfBytesGenerator(IDigest digest) : base(0, digest)
        {
        }

        public override void Init(IDerivationParameters parameters)
        {
            KdfParameters kdfParameters = (KdfParameters) parameters;
            if (kdfParameters != null)
            {
                _shared = kdfParameters.GetSharedSecret();
            }
            else
            {
                throw new ArgumentException("KDF Parameters Required For KDF Generator");
            }
        }

        public override int GenerateBytes(byte[] output, int outOff, int length)
        {
            if ((output.Length - length) < outOff)
            {
                throw new DataLengthException("Output Buffer too Small");
            }

            int outLen = Digest.GetDigestSize();

            if (length > outLen)
            {
                throw new DataLengthException(
                    "Specified Hash Cannot Produce Sufficient Data for the Specified Operation.");
            }

            byte[] temp = new byte[Digest.GetDigestSize()];

            Digest.BlockUpdate(_shared, 0, _shared.Length);
            Digest.DoFinal(temp, 0);

            Array.Copy(temp, 0, output, outOff, length);

            Digest.Reset();
            return length;
        }
    }
}