// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using NUnit.Framework.Legacy;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Hydrogen.CryptoEx.Tests;

/* This class is a modified version of the BouncyCastle Cryptographic Library Test class "DigestTest" and was borrowed from 
<a href="https://github.com/bcgit/bc-csharp/blob/master/crypto/test/src/test/DigestTest.cs">https://github.com/bcgit/bc-csharp/blob/master/crypto/test/src/test/DigestTest.cs</a>
for internal use.
*/
public abstract class DigestTest {
	private readonly IDigest _digest;
	private readonly string[] _input;
	private readonly string[] _results;

	protected DigestTest(
		IDigest digest,
		string[] input,
		string[] results) {
		_digest = digest;
		_input = input;
		_results = results;
	}

	protected virtual void PerformTest() {
		byte[] resBuf = new byte[_digest.GetDigestSize()];

		for (int i = 0; i < _input.Length - 1; i++) {
			byte[] msg = toByteArray(_input[i]);

			vectorTest(_digest, i, resBuf, msg, Hex.Decode(_results[i]));
		}

		byte[] lastV = toByteArray(_input[_input.Length - 1]);
		byte[] lastDigest = Hex.Decode(_results[_input.Length - 1]);

		vectorTest(_digest, _input.Length - 1, resBuf, lastV, Hex.Decode(_results[_input.Length - 1]));

		//
		// clone test
		//
		_digest.BlockUpdate(lastV, 0, lastV.Length / 2);

		// clone the Digest
		IDigest d = CloneDigest(_digest);

		_digest.BlockUpdate(lastV, lastV.Length / 2, lastV.Length - lastV.Length / 2);
		_digest.DoFinal(resBuf, 0);

		ClassicAssert.AreEqual(lastDigest,
			resBuf,
			string.Format("fail clone vector test, expected {0} but got {1}",
				_results[_results.Length - 1],
				Hex.ToHexString(resBuf)));

		d.BlockUpdate(lastV, lastV.Length / 2, lastV.Length - lastV.Length / 2);
		d.DoFinal(resBuf, 0);

		ClassicAssert.AreEqual(lastDigest,
			resBuf,
			string.Format("fail second clone vector test, expected {0} but got {1}",
				_results[_results.Length - 1],
				Hex.ToHexString(resBuf)));

		//
		// memo test
		//
		IMemoable m = (IMemoable)_digest;

		_digest.BlockUpdate(lastV, 0, lastV.Length / 2);

		// copy the Digest
		IMemoable copy1 = m.Copy();
		IMemoable copy2 = copy1.Copy();

		_digest.BlockUpdate(lastV, lastV.Length / 2, lastV.Length - lastV.Length / 2);
		_digest.DoFinal(resBuf, 0);

		ClassicAssert.AreEqual(lastDigest,
			resBuf,
			string.Format("fail memo vector test, expected {0} but got {1}",
				_results[_results.Length - 1],
				Hex.ToHexString(resBuf)));

		m.Reset(copy1);

		_digest.BlockUpdate(lastV, lastV.Length / 2, lastV.Length - lastV.Length / 2);
		_digest.DoFinal(resBuf, 0);

		ClassicAssert.AreEqual(lastDigest,
			resBuf,
			string.Format("fail memo reset vector test, expected {0} but got {1}",
				_results[_results.Length - 1],
				Hex.ToHexString(resBuf)));

		IDigest md = (IDigest)copy2;

		md.BlockUpdate(lastV, lastV.Length / 2, lastV.Length - lastV.Length / 2);
		md.DoFinal(resBuf, 0);

		ClassicAssert.AreEqual(lastDigest,
			resBuf,
			string.Format("fail memo copy vector test, expected {0} but got {1}",
				_results[_results.Length - 1],
				Hex.ToHexString(resBuf)));
	}

	private byte[] toByteArray(
		string aInput) {
		byte[] bytes = new byte[aInput.Length];

		for (int i = 0; i != bytes.Length; i++) {
			bytes[i] = (byte)aInput[i];
		}

		return bytes;
	}

	private void vectorTest(
		IDigest aDigest,
		int count,
		byte[] resBuf,
		byte[] aInput,
		byte[] expected) {
		aDigest.BlockUpdate(aInput, 0, aInput.Length);
		aDigest.DoFinal(resBuf, 0);

		ClassicAssert.AreEqual(resBuf,
			expected,
			string.Format("Vector {0} failed, got {1}", count, Hex.ToHexString(resBuf)));
	}

	protected abstract IDigest CloneDigest(IDigest digest);
}
