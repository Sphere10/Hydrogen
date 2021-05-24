using System;
using NUnit.Framework;
using Sphere10.Framework.CryptoEx.EC;
using System.Linq;
using System.Text;
using Tools;

namespace Sphere10.Framework.CryptoEx.Tests {

	[TestFixture]
	public class ECIESTests {

		private static readonly Random Random = new();
		private static string RandomString(int length) {
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[Random.Next(s.Length)]).ToArray());
		}

		[Test]
		[TestCase(ECDSAKeyType.SECP256K1)]
		[TestCase(ECDSAKeyType.SECP384R1)]
		[TestCase(ECDSAKeyType.SECP521R1)]
		[TestCase(ECDSAKeyType.SECT283K1)]
		public void EncryptDecrypt_Basic(ECDSAKeyType keyType) {
			var ecdsa = new ECDSA(keyType);
			var secret = new byte[] { 0, 1, 2, 3, 4 }; // deterministic secret should derive deterministic key!
			var privateKey = ecdsa.GeneratePrivateKey();
			var publicKey = ecdsa.DerivePublicKey(privateKey);
			var message = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
			var encryptedData = ecdsa.IES.Encrypt(message, publicKey);
			Assert.IsTrue(ecdsa.IES.TryDecrypt(encryptedData, out var decryptedData, privateKey));
			Assert.IsTrue(message.SequenceEqual(decryptedData.ToArray()));
		}

		[Test]
		[Repeat(1000)]
		[TestCase(ECDSAKeyType.SECP256K1)]
		[TestCase(ECDSAKeyType.SECP384R1)]
		[TestCase(ECDSAKeyType.SECP521R1)]
		[TestCase(ECDSAKeyType.SECT283K1)]
		public void EncryptDecrypt_Random(ECDSAKeyType keyType) {
			var ecdsa = new ECDSA(keyType);
			var secret = Crypto.GenerateCryptographicallyRandomBytes(Random.Next(5, 64));
			var privateKey = ecdsa.GeneratePrivateKey();
			var publicKey = ecdsa.DerivePublicKey(privateKey);
			var message = Encoding.ASCII.GetBytes(RandomString(Random.Next(1, 1000)));
			var encryptedData = ecdsa.IES.Encrypt(message, publicKey);
			Assert.IsTrue(ecdsa.IES.TryDecrypt(encryptedData, out var decryptedData, privateKey));
			Assert.IsTrue(message.SequenceEqual(decryptedData.ToArray()));
		}

	}

}
