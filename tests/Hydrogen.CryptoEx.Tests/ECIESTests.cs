// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;
using Hydrogen.CryptoEx.EC;
using System.Linq;
using System.Text;
using NUnit.Framework.Legacy;
using Tools;

namespace Hydrogen.CryptoEx.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ECIESTests {

	private static string RandomString(int length, int seed) {
		Random Random = new(seed);
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
		ClassicAssert.IsTrue(ecdsa.IES.TryDecrypt(encryptedData, out var decryptedData, privateKey));
		ClassicAssert.IsTrue(message.SequenceEqual(decryptedData.ToArray()));
	}

	[Test]
	[TestCase(ECDSAKeyType.SECP256K1, 1000)]
	[TestCase(ECDSAKeyType.SECP384R1, 1000)]
	[TestCase(ECDSAKeyType.SECP521R1, 1000)]
	[TestCase(ECDSAKeyType.SECT283K1, 1000)]
	public void EncryptDecrypt_Random(ECDSAKeyType keyType, int iterations) {
		var rng = new Random(31337 * iterations);
		var ecdsa = new ECDSA(keyType);
		var secret = Crypto.GenerateCryptographicallyRandomBytes(rng.Next(5, 64));
		var privateKey = ecdsa.GeneratePrivateKey();
		var publicKey = ecdsa.DerivePublicKey(privateKey);
		var message = Encoding.ASCII.GetBytes(RandomString(rng.Next(1, 1000), iterations));
		var encryptedData = ecdsa.IES.Encrypt(message, publicKey);
		ClassicAssert.IsTrue(ecdsa.IES.TryDecrypt(encryptedData, out var decryptedData, privateKey));
		ClassicAssert.IsTrue(message.SequenceEqual(decryptedData.ToArray()));
	}

}
