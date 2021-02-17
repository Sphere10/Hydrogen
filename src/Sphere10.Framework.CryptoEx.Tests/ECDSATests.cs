using System.Text;
using NUnit.Framework;

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
            var ecdsa = new ECDSA.ECDSA(keyType);
            var secret = ECDSA.ECDSA.PrivateKey.DoGetRandomPrivateKey(keyType);
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
            var ecdsa = new ECDSA.ECDSA(keyType);
            var secret = ECDSA.ECDSA.PrivateKey.DoGetRandomPrivateKey(keyType);
            var privateKey = ecdsa.GeneratePrivateKey(secret);
            var publicKey = ecdsa.DerivePublicKey(privateKey);
            Assert.IsTrue(ecdsa.IsPublicKey(privateKey, publicKey.RawBytes));
        }

        [Test]
        [TestCase(new byte[] { }, ECDSAKeyType.SECP256K1)]
        [TestCase(new byte[] {0, 0}, ECDSAKeyType.SECP256K1)]
        [TestCase(new byte[] { }, ECDSAKeyType.SECP384R1)]
        [TestCase(new byte[] {0, 0}, ECDSAKeyType.SECP384R1)]
        [TestCase(new byte[] { }, ECDSAKeyType.SECP521R1)]
        [TestCase(new byte[] {0, 0}, ECDSAKeyType.SECP521R1)]
        [TestCase(new byte[] { }, ECDSAKeyType.SECT283K1)]
        [TestCase(new byte[] {0, 0}, ECDSAKeyType.SECT283K1)]
        public void VerifyThatTryParsePrivateKeyFailsEarlyForBadKeys(byte[] badRawKey, ECDSAKeyType keyType)
        {
            Assert.IsFalse(ECDSA.ECDSA.PrivateKey.TryParse(badRawKey, keyType, out var privateKey));
            if (ECDSA.ECDSA.PrivateKey.KeyTypeOrders.TryGetValue(keyType, out var order))
            {
                Assert.IsFalse(ECDSA.ECDSA.PrivateKey.TryParse(order.ToByteArrayUnsigned(), keyType,
                    out privateKey));
            }
        }
    }

}