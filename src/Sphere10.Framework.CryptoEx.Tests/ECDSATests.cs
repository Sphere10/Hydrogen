using System.Text;
using NUnit.Framework;
using Sphere10.Framework.CryptoEx;

namespace Sphere10.Framework.CryptoEx.Tests
{

    [TestFixture]
    public class ECDSATests
    {
        [Test]
        [TestCase(ECDSAKeyType.SECP256K1)]
        [TestCase(ECDSAKeyType.SECP384R1)]
        [TestCase(ECDSAKeyType.SECP521R1)]
        [TestCase(ECDSAKeyType.SECT283K1)]
        public void SignVerify_Basic(ECDSAKeyType keyType)
        {
            var ecdsa = new ECDSA(keyType);
            var secret = ECDSAUtils.DoGetRandomPrivateKey(keyType);
            var privateKey = ecdsa.GeneratePrivateKey(secret);
            var publicKey = ecdsa.DerivePublicKey(privateKey);

            var message = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
            var sig = ecdsa.Sign(privateKey, message);
            Assert.IsTrue(ecdsa.Verify(sig, message, publicKey));
        }

        [Test]
        [TestCase(ECDSAKeyType.SECP256K1)]
        [TestCase(ECDSAKeyType.SECP384R1)]
        [TestCase(ECDSAKeyType.SECP521R1)]
        [TestCase(ECDSAKeyType.SECT283K1)]
        public void IsPublicKey(ECDSAKeyType keyType)
        {
            var ecdsa = new ECDSA(keyType);
            var secret = ECDSAUtils.DoGetRandomPrivateKey(keyType);
            var privateKey = ecdsa.GeneratePrivateKey(secret);
            var publicKey = ecdsa.DerivePublicKey(privateKey);
            Assert.IsTrue(ecdsa.IsPublicKey(privateKey, publicKey.RawBytes));
        }
    }

}