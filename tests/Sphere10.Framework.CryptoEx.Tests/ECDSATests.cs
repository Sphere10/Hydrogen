using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Sphere10.Framework.CryptoEx.EC;
using System.Text;
using Tools;
using System.Reflection;
using System.IO;

namespace Sphere10.Framework.CryptoEx.Tests {

	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class ECDSATests {
		private static string RandomString(int length) {
			var rng = new Random(31337);
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[rng.Next(s.Length)]).ToArray());
		}

		[Test]
		[TestCase(ECDSAKeyType.SECP256K1)]
		[TestCase(ECDSAKeyType.SECP384R1)]
		[TestCase(ECDSAKeyType.SECP521R1)]
		[TestCase(ECDSAKeyType.SECT283K1)]
		public void SignVerify_Basic(ECDSAKeyType keyType) {
			var ecdsa = new ECDSA(keyType);
			var secret = new byte[] { 0, 1, 2, 3, 4 }; // deterministic secret
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
		public void IsPublicKey(ECDSAKeyType keyType) {
			var ecdsa = new ECDSA(keyType);
			var secret = new byte[] { 0, 1, 2, 3, 4 }; // deterministic secret
			var privateKey = ecdsa.GeneratePrivateKey(secret);
			var publicKey = ecdsa.DerivePublicKey(privateKey);
			Assert.IsTrue(ecdsa.IsPublicKey(privateKey, publicKey.RawBytes));
		}

		[Test]
		[TestCase(new byte[] { }, ECDSAKeyType.SECP256K1)]
		[TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECP256K1)]
		[TestCase(new byte[] { }, ECDSAKeyType.SECP384R1)]
		[TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECP384R1)]
		[TestCase(new byte[] { }, ECDSAKeyType.SECP521R1)]
		[TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECP521R1)]
		[TestCase(new byte[] { }, ECDSAKeyType.SECT283K1)]
		[TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECT283K1)]
		public void VerifyThatTryParsePrivateKeyFailsEarlyForBadKeys(byte[] badRawKey, ECDSAKeyType keyType) {
			var ecdsa = new ECDSA(keyType);
			Assert.IsFalse(ecdsa.TryParsePrivateKey(badRawKey, out _));
			var order = keyType.GetAttribute<KeyTypeOrderAttribute>().Value;
			Assert.IsFalse(ecdsa.TryParsePrivateKey(order.ToByteArrayUnsigned(), out _));
		}

	
	}

}
