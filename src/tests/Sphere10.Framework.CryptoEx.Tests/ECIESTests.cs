using System.Linq;
using System.Text;
using NUnit.Framework;
using Sphere10.Framework.CryptoEx.EC;
using Sphere10.Framework.CryptoEx.EC.IES;
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
            var ecdsa = new ECDSA(keyType);
            var privateKey = ecdsa.GeneratePrivateKey(new byte[] {0, 1, 2, 3, 4});   // deterministic secret should derive deterministic key!
            var publicKey = ecdsa.DerivePublicKey(privateKey);
            var message = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
            var encryptedData = ecdsa.IES.Encrypt(message, publicKey);
            Assert.IsTrue(ecdsa.IES.TryDecrypt(encryptedData, out var decryptedData, privateKey));
            Assert.IsTrue(message.SequenceEqual(decryptedData.ToArray()));
        }
    }

}