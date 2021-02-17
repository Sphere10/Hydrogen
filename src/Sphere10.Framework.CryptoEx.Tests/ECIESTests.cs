using System.Linq;
using System.Text;
using NUnit.Framework;
using Sphere10.Framework.CryptoEx.IES;

namespace Sphere10.Framework.CryptoEx.Tests
{

    [TestFixture]
    public class ECIESTests
    {
        [Test]
        [TestCase(ECDSAKeyType.SECP256K1)]
        [TestCase(ECDSAKeyType.SECP384R1)]
        [TestCase(ECDSAKeyType.SECP521R1)]
        [TestCase(ECDSAKeyType.SECT283K1)]
        public void EncryptDecrypt_Basic(ECDSAKeyType keyType)
        {
            var ecdsa = new ECDSA.ECDSA(keyType);
            var secret = ECDSA.ECDSA.PrivateKey.DoGetRandomPrivateKey(keyType);
            var privateKey = ecdsa.GeneratePrivateKey(secret);
            var publicKey = ecdsa.DerivePublicKey(privateKey);
            var ecies = new ECIES();
            var message = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
            var encryptedData = ecies.Encrypt(message, publicKey);
            Assert.IsTrue(ecies.TryDecrypt(encryptedData, out var decryptedData, privateKey));
            Assert.IsTrue(message.SequenceEqual(decryptedData.ToArray()));
        }
    }

}