// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public abstract class HashFunctionBase : IHashFunction {
	protected bool _inTransform;
	private byte[] _computedDigest;

	public abstract int DigestSize { get; }

	public virtual void Initialize() {
		_computedDigest = null; // Important not set this value since Transform needs to know when "empty pre-image" usecase
	}

	public virtual void Compute(ReadOnlySpan<byte> input, Span<byte> output) {
		// Standard single transformation, sub-class can override for performance
		Guard.Ensure(!_inTransform, "Complete prior transformations before starting a new one");
		Transform(input);
		GetResult(output);
	}

	public virtual void Transform(ReadOnlySpan<byte> data) {
		if (!_inTransform) {
			Initialize();
			_inTransform = true;
		}
		// Super-class implementation will perform the hashing
	}

	protected abstract void Finalize(Span<byte> digest);

	public void GetResult(Span<byte> result) {
		if (_inTransform || _computedDigest is null) {
			if (_computedDigest == null) //  _computedDigest == null is the case when no Transform has been applied, and thus is the "empty pre-image" use-case
				_computedDigest = new byte[DigestSize];
			else if (_computedDigest.Length != DigestSize) // _computedDigest was from previous pre-image, resize it
				Array.Resize(ref _computedDigest, DigestSize);
			_inTransform = false;
			Finalize(_computedDigest);
		}
		_computedDigest.CopyTo(result);
	}

	public void Reset() {
		Initialize();
		_computedDigest = null;
	}

	public abstract object Clone();

	public virtual void Dispose() {
	}


}
