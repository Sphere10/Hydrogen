// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Hydrogen.CryptoEx.EC;
using System.Text;
using Tools;
using System.IO;
using NUnit.Framework.Legacy;

namespace Hydrogen.CryptoEx.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
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

	[SetUp]
	public void InitTest() {
		Assume.That(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), Is.Not.EqualTo("true"), "Test skipped in GitHub Actions.");
		Assume.That(Environment.OSVersion.Platform, Is.EqualTo(PlatformID.Win32NT), "Test skipped on non-Windows platforms.");
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
	public void TestThatWeCanVerifyOpenSSLSignatures([Values] ECDSAKeyType keyType, [Range(1, 100)] int seed) {
		var rng = new Random(seed * 31337);
		var ecdsa = new ECDSA(keyType);
		var secret = Crypto.GenerateCryptographicallyRandomBytes(rng.Next(5, 64));
		var privateKey = ecdsa.GeneratePrivateKey(secret);
		var privateKeyAsHex = privateKey.Parameters.Value.D.ToByteArray().ToHexString(true);
		var publicKey = ecdsa.DerivePublicKey(privateKey);
		var curveName = Enum.GetName(typeof(ECDSAKeyType), keyType);
		var message = Encoding.ASCII.GetBytes(RandomString(rng.Next(1, 1000)));
		var messageDigest = Hashers.Hash(CHF.SHA2_256, message).ToHexString(true);
		var order = privateKey.Parameters.Value.Parameters.Curve.Order;
		string[] args = {
			"-operationtype SIGN",
			$"-curvetype {curveName}",
			$"-messagedigest {messageDigest}",
			$"-privatekey {privateKeyAsHex}"
		};

		var sig = CallPascalOpenSSL(args).ToHexByteArray();
		// OpenSSL doesn't take into account the "LowS fix" to resolve signature malleability so we account for it here
		sig = ECDSATests.CanonicalizeSig(order, sig);
		ClassicAssert.IsTrue(ecdsa.VerifyDigest(sig, messageDigest.ToHexByteArray(), publicKey));
	}

	[Test]
	public void TestThatOpenSSLCanVerifyOurSignatures([Values] ECDSAKeyType keyType, [Range(1, 100)] int seed) {
		var rng = new Random(seed * 31337);
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
		ClassicAssert.IsTrue(isValidSig.ToBool());
	}


	[Test]
	public void TestThatOpenSSLDoesNotVerifyBadSignatures([Values] ECDSAKeyType keyType, [Range(1, 100)] int seed) {
		var rng = new Random(seed * 31337);
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
		var badDerSig = ecdsa.SignDigest(privateKey, messageDigest); // so far signature is good
		unchecked {
			var bitArray = new BitArray(badDerSig);
			// increment a random byte in the signature
			var index = rng.Next(0, bitArray.Length);
			bitArray[index] = !bitArray[index];
			bitArray.CopyTo(badDerSig, 0);
		}
		string[] args = {
			"-operationtype VERIFY",
			$"-curvetype {curveName}",
			$"-messagedigest {messageDigestAsHex}",
			$"-xCoord {xCoord}",
			$"-yCoord {yCoord}",
			$"-derSig {badDerSig}",
		};

		var isValidSig = CallPascalOpenSSL(args);
		ClassicAssert.IsFalse(isValidSig.ToBool());
	}
}
