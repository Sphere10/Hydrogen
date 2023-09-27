// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Org.BouncyCastle.Crypto.Digests;

namespace Hydrogen.CryptoEx;

// Needs Testing (RaceConditions)
public class GeneralDigestAdapter : IHashFunction {
	private readonly GeneralDigest _generalDigest;
	private bool _inTransform = false;
	private static readonly byte[] NullBytes = Array.Empty<byte>();

	public GeneralDigestAdapter(GeneralDigest generalDigest) {
		_generalDigest = generalDigest;
		DigestSize = generalDigest.GetDigestSize();
	}

	public int DigestSize { get; }


	public void Compute(ReadOnlySpan<byte> input, Span<byte> output) {
		if (_inTransform)
			throw new InvalidOperationException("Complete prior transformations before starting a new one");
		Transform(input);
		GetResult(output);
	}

	public void Transform(ReadOnlySpan<byte> part) {
		_inTransform = true;
		_generalDigest.BlockUpdate(part.ToArray(), 0, part.Length);
	}

	public void GetResult(Span<byte> result) {
		if (!_inTransform)
			throw new InvalidOperationException("Nothing was transformed");
		try {
			var outputArr = new byte[DigestSize];
			_generalDigest.DoFinal(outputArr, 0);
			outputArr.CopyTo(result);
		} finally {
			_generalDigest.Reset();
			_inTransform = false;
		}
	}

	public void Reset() {
		_generalDigest.Reset();
	}

	public void Dispose() {
	}


	public object Clone() {
		throw new NotSupportedException();
	}

}
