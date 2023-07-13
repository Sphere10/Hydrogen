// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using HashLib4CSharp.Interfaces;

namespace Hydrogen;

public class HashLibAdapter : IHashFunction {
	private readonly IHash _hashAlgorithm;
	private bool _needsFinalBlock;
	private IHashResult _hashResult;

	public HashLibAdapter(IHash hashAlgorithm) {
		_hashAlgorithm = hashAlgorithm;
		_hashAlgorithm.Initialize();
		DigestSize = hashAlgorithm.HashSize;
		_hashResult = null;

	}

	public int DigestSize { get; }

	public void Compute(ReadOnlySpan<byte> input, Span<byte> output) {
		if (_needsFinalBlock)
			throw new InvalidOperationException("Complete prior transformations before starting a new one");
		// _hashResult.ComputeBytes(input.ToArray()).CopyTo(result);  // Add when Ugo adds
		_hashAlgorithm.ComputeBytes(input.ToArray()).GetBytes().CopyTo(output);
	}

	public void Transform(ReadOnlySpan<byte> part) {
		_needsFinalBlock = true;
		_hashAlgorithm.TransformByteSpan(part);
	}

	public void GetResult(Span<byte> result) {
		if (_needsFinalBlock || _hashResult is null) {
			_needsFinalBlock = false;
			_hashResult = _hashAlgorithm.TransformFinal();
			_hashAlgorithm.Initialize();
		}
		// _hashResult.CopyTo(result);  // Add when Ugo adds
		_hashResult.GetBytes().CopyTo(result);
	}

	public void Reset() {
		_hashAlgorithm.Initialize();
		_hashResult = null;
	}

	public void Dispose() {
	}

	public object Clone() {
		return _hashAlgorithm.Clone();
	}


}
