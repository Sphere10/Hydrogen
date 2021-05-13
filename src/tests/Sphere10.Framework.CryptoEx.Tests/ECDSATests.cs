using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Sphere10.Framework.CryptoEx.EC;
using System.Text;
using Tools;

namespace Sphere10.Framework.CryptoEx.Tests {

	[TestFixture]
	public class ECDSATests {

		[OneTimeSetUp]
		public void Init() {
			// extract the needed Binaries (OpenSSL dlls and PascalOpenSSL.exe) and assign path to variable below
			_pascalOpenSslFilePath = @"C:\Kam\PascalOpenSSL.exe";
		}

		[OneTimeTearDown]
		public void Cleanup() {
			// Delete the extracted resources here
		}

		private static readonly Random Random = new();
		private static string _pascalOpenSslFilePath;
		private static string CallPascalOpenSSL(string[] args) {
			var startInfo = new ProcessStartInfo {
				FileName = _pascalOpenSslFilePath,
				UseShellExecute = false,
				Arguments = string.Join(" ", args),
				CreateNoWindow = true,
				RedirectStandardOutput = true
			};

			using (var process = Process.Start(startInfo)) {
				process?.WaitForExit();
				using (var reader = process?.StandardOutput) {
					var result = reader?.ReadToEnd();
					return result?.Trim();
				}
			}
		}

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

		[Test]
		[Ignore("Ignore this test till we are able to embed the PascalOpenSSL exe in resources and get a definite path for it.")]
		[Repeat(1000)]
		[TestCase(ECDSAKeyType.SECP256K1)]
		[TestCase(ECDSAKeyType.SECP384R1)]
		[TestCase(ECDSAKeyType.SECP521R1)]
		[TestCase(ECDSAKeyType.SECT283K1)]
		public void TestThatWeCanVerifyOpenSSLSignatures(ECDSAKeyType keyType) {
			var ecdsa = new ECDSA(keyType);
			var secret = Crypto.GenerateCryptographicallyRandomBytes(Random.Next(5, 64));
			var privateKey = ecdsa.GeneratePrivateKey(secret);
			var privateKeyAsHex = privateKey.Parameters.Value.D.ToByteArray().ToHexString(true);
			var publicKey = ecdsa.DerivePublicKey(privateKey);
			var curveName = Enum.GetName(typeof(ECDSAKeyType), keyType);
			var message = Encoding.ASCII.GetBytes(RandomString(Random.Next(1, 1000)));
			var messageDigest = Hashers.Hash(CHF.SHA2_256, message).ToHexString(true);
			string[] args = {
				"-operationtype SIGN",
				$"-curvetype {curveName}",
				$"-messagedigest {messageDigest}",
				$"-privatekey {privateKeyAsHex}"
			};

			var sig = CallPascalOpenSSL(args).ToHexByteArray();
			Assert.IsTrue(ecdsa.Verify(sig, message, publicKey));
		}

		[Test]
		[Ignore("Ignore this test till we are able to embed the PascalOpenSSL exe in resources and get a definite path for it.")]
		[Repeat(1000)]
		[TestCase(ECDSAKeyType.SECP256K1)]
		[TestCase(ECDSAKeyType.SECP384R1)]
		[TestCase(ECDSAKeyType.SECP521R1)]
		[TestCase(ECDSAKeyType.SECT283K1)]
		public void TestThatOpenSSLCanVerifyOurSignatures(ECDSAKeyType keyType) {
			var ecdsa = new ECDSA(keyType);
			var secret = Crypto.GenerateCryptographicallyRandomBytes(Random.Next(5, 64));
			var privateKey = ecdsa.GeneratePrivateKey(secret);
			var publicKeyPoint = ecdsa.DerivePublicKey(privateKey).Parameters.Value.Q;
			var xCoord = publicKeyPoint.AffineXCoord.ToBigInteger().ToByteArray().ToHexString(true);
			var yCoord = publicKeyPoint.AffineYCoord.ToBigInteger().ToByteArray().ToHexString(true);
			var curveName = Enum.GetName(typeof(ECDSAKeyType), keyType);
			var message = Encoding.ASCII.GetBytes(RandomString(Random.Next(1, 1000)));
			var messageDigest = Hashers.Hash(CHF.SHA2_256, message);
			var messageDigestAsHex = messageDigest.ToHexString(true);
			var derSig = ecdsa.SignDigest(privateKey, messageDigest).ToHexString(true);
			string[] args = {
				"-operationtype VERIFY",
				$"-curvetype {curveName}",
				$"-messagedigest {messageDigestAsHex}",
				$"-xCoord {xCoord}",
				$"-yCoord {yCoord}",
				$"-derSig {derSig}",
			};

			var isValidSig = CallPascalOpenSSL(args);
			Assert.IsTrue(isValidSig.ToBool());
		}
		
		[Test]
		[Ignore("Ignore this test till we are able to embed the PascalOpenSSL exe in resources and get a definite path for it.")]
		[Repeat(1000)]
		[TestCase(ECDSAKeyType.SECP256K1)]
		[TestCase(ECDSAKeyType.SECP384R1)]
		[TestCase(ECDSAKeyType.SECP521R1)]
		[TestCase(ECDSAKeyType.SECT283K1)]
		public void TestThatOpenSSLDoesNotVerifyBadSignatures(ECDSAKeyType keyType) {
			var ecdsa = new ECDSA(keyType);
			var secret = Crypto.GenerateCryptographicallyRandomBytes(Random.Next(5, 64));
			var privateKey = ecdsa.GeneratePrivateKey(secret);
			var publicKeyPoint = ecdsa.DerivePublicKey(privateKey).Parameters.Value.Q;
			var xCoord = publicKeyPoint.AffineXCoord.ToBigInteger().ToByteArray().ToHexString(true);
			var yCoord = publicKeyPoint.AffineYCoord.ToBigInteger().ToByteArray().ToHexString(true);
			var curveName = Enum.GetName(typeof(ECDSAKeyType), keyType);
			var message = Encoding.ASCII.GetBytes(RandomString(Random.Next(1, 1000)));
			var messageDigest = Hashers.Hash(CHF.SHA2_256, message);
			var messageDigestAsHex = messageDigest.ToHexString(true);
			var badDerSig = Crypto.GenerateCryptographicallyRandomBytes(Random.Next(1, 254));
			string[] args = {
				"-operationtype VERIFY",
				$"-curvetype {curveName}",
				$"-messagedigest {messageDigestAsHex}",
				$"-xCoord {xCoord}",
				$"-yCoord {yCoord}",
				$"-derSig {badDerSig}",
			};

			var isValidSig = CallPascalOpenSSL(args);
			Assert.IsFalse(isValidSig.ToBool());
		}
	}

}
