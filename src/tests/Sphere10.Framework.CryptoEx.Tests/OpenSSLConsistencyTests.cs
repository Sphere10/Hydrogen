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
	//[Platform("Windows", Reason = "PascalOpenSSL is only supported on Windows platforms")]
	public class OpenSSLConsistencyTests {
		private string _pascalOpenSSLFolder;
		private string _pascalOpenSslFilePath;

		[OneTimeSetUp]
		public void Init() {
			// extract the needed Binaries (OpenSSL dlls and PascalOpenSSL.exe) and assign path to variable below
			_pascalOpenSSLFolder = Tools.FileSystem.GetTempEmptyDirectory(true);
			Tools.FileSystem.AppendAllBytes(Path.Combine(_pascalOpenSSLFolder, "libcrypto-1_1.dll"), Properties.Resource.libcrypto_1_1_dll);
			Tools.FileSystem.AppendAllBytes(Path.Combine(_pascalOpenSSLFolder, "libcrypto-1_1-x64.dll"), Properties.Resource.libcrypto_1_1_x64_dll);
			Tools.FileSystem.AppendAllBytes(Path.Combine(_pascalOpenSSLFolder, "PascalOpenSSL.exe"), Properties.Resource.PascalOpenSSL_exe);
			_pascalOpenSslFilePath = Path.Combine(_pascalOpenSSLFolder, "PascalOpenSSL.exe");
		}

		[OneTimeTearDown]
		public void Cleanup() {
			// Delete the extracted resources here
			Tools.FileSystem.DeleteDirectory(_pascalOpenSSLFolder);
		}

		private string CallPascalOpenSSL(string[] args) {
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
			var rng = new Random(31337);
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[rng.Next(s.Length)]).ToArray());
		}


		[Test]
		[Ignore("Ignore this test till we are able to embed the PascalOpenSSL exe in resources and get a definite path for it.")]
		[Repeat(1000)]
		[TestCase(ECDSAKeyType.SECP256K1)]
		[TestCase(ECDSAKeyType.SECP384R1)]
		[TestCase(ECDSAKeyType.SECP521R1)]
		[TestCase(ECDSAKeyType.SECT283K1)]
		public void TestThatWeCanVerifyOpenSSLSignatures(ECDSAKeyType keyType) {
			var rng = new Random(31337);
			var ecdsa = new ECDSA(keyType);
			var secret = Crypto.GenerateCryptographicallyRandomBytes(rng.Next(5, 64));
			var privateKey = ecdsa.GeneratePrivateKey(secret);
			var privateKeyAsHex = privateKey.Parameters.Value.D.ToByteArray().ToHexString(true);
			var publicKey = ecdsa.DerivePublicKey(privateKey);
			var curveName = Enum.GetName(typeof(ECDSAKeyType), keyType);
			var message = Encoding.ASCII.GetBytes(RandomString(rng.Next(1, 1000)));
			var messageDigest = Hashers.Hash(CHF.SHA2_256, message).ToHexString(true);
			string[] args = {
				"-operationtype SIGN",
				$"-curvetype {curveName}",
				$"-messagedigest {messageDigest}",
				$"-privatekey {privateKeyAsHex}"
			};

			var sig = CallPascalOpenSSL(args).ToHexByteArray();
			Assert.IsTrue(ecdsa.VerifyDigest(sig, messageDigest.ToHexByteArray(), publicKey));
		}

		[Test]
		[Ignore("Ignore this test till we are able to embed the PascalOpenSSL exe in resources and get a definite path for it.")]
		[Repeat(1000)]
		[TestCase(ECDSAKeyType.SECP256K1)]
		[TestCase(ECDSAKeyType.SECP384R1)]
		[TestCase(ECDSAKeyType.SECP521R1)]
		[TestCase(ECDSAKeyType.SECT283K1)]
		public void TestThatOpenSSLCanVerifyOurSignatures(ECDSAKeyType keyType) {
			var rng = new Random(31337);
			var ecdsa = new ECDSA(keyType);
			var secret = Crypto.GenerateCryptographicallyRandomBytes(rng.Next(5, 64));
			var privateKey = ecdsa.GeneratePrivateKey(secret);
			var publicKeyPoint = ecdsa.DerivePublicKey(privateKey).Parameters.Value.Q;
			var xCoord = publicKeyPoint.AffineXCoord.ToBigInteger().ToByteArray().ToHexString(true);
			var yCoord = publicKeyPoint.AffineYCoord.ToBigInteger().ToByteArray().ToHexString(true);
			var curveName = Enum.GetName(typeof(ECDSAKeyType), keyType);
			var message = Encoding.ASCII.GetBytes(RandomString(rng.Next(1, 1000)));
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
			var rng = new Random(31337);
			var ecdsa = new ECDSA(keyType);
			var secret = Crypto.GenerateCryptographicallyRandomBytes(rng.Next(5, 64));
			var privateKey = ecdsa.GeneratePrivateKey(secret);
			var publicKeyPoint = ecdsa.DerivePublicKey(privateKey).Parameters.Value.Q;
			var xCoord = publicKeyPoint.AffineXCoord.ToBigInteger().ToByteArray().ToHexString(true);
			var yCoord = publicKeyPoint.AffineYCoord.ToBigInteger().ToByteArray().ToHexString(true);
			var curveName = Enum.GetName(typeof(ECDSAKeyType), keyType);
			var message = Encoding.ASCII.GetBytes(RandomString(rng.Next(1, 1000)));
			var messageDigest = Hashers.Hash(CHF.SHA2_256, message);
			var messageDigestAsHex = messageDigest.ToHexString(true);
			var badDerSig = Crypto.GenerateCryptographicallyRandomBytes(rng.Next(1, 254));
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
