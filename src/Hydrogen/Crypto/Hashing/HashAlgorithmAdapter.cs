// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Security.Cryptography;

namespace Hydrogen;

public class HashAlgorithmAdapter : IHashFunction {
	private readonly HashAlgorithm _hashAlgorithm;
	private bool _inTransform = false;
	private bool _computedDigest;
	private static readonly byte[] NullBytes = Array.Empty<byte>();


	public HashAlgorithmAdapter(HashAlgorithm hashAlgorithm) {
		Guard.ArgumentNotNull(hashAlgorithm, nameof(hashAlgorithm));
		Guard.Argument(hashAlgorithm.CanTransformMultipleBlocks, nameof(hashAlgorithm), "Must support transformation of multiple blocks");
		Guard.Argument(hashAlgorithm.CanReuseTransform, nameof(hashAlgorithm), "Must support transformation reuse");
		_hashAlgorithm = hashAlgorithm;
		DigestSize = hashAlgorithm.HashSize >> 3;
		_computedDigest = false;
		_inTransform = false;
	}

	public int DigestSize { get; }

	public void Compute(ReadOnlySpan<byte> input, Span<byte> output) {
		// Some .NET HashAlgorithm's default to native vectorized extern's (FAST)
		if (_inTransform)
			throw new InvalidOperationException("Complete prior transformations before starting a new one");

		if (!_hashAlgorithm.TryComputeHash(input, output, out _))
			throw new InvalidOperationException();
	}

	public void Transform(ReadOnlySpan<byte> part) {
		// Will always use managed transform (Slower)
		_inTransform = true;
		var arr = part.ToArray();
		_hashAlgorithm.TransformBlock(arr, 0, arr.Length, null, 0);
	}

	public void GetResult(Span<byte> result) {
		if (_inTransform || !_computedDigest) {
			_hashAlgorithm.TransformFinalBlock(NullBytes, 0, 0);
			_inTransform = false;
			_computedDigest = true;
		}
		_hashAlgorithm.Hash.CopyTo(result);
	}

	public void Reset() {
		_hashAlgorithm.Initialize();
		_inTransform = false;
		_computedDigest = false;
	}

	public void Dispose() {
		_hashAlgorithm.Dispose();
	}

	public object Clone() {
		throw new NotSupportedException();
	}

}
