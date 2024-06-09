// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.CompilerServices;

namespace Hydrogen;

public interface IHashFunction : ICloneable, IDisposable {

	int DigestSize { get; }

	void Compute(ReadOnlySpan<byte> input, Span<byte> output);

	void Transform(ReadOnlySpan<byte> data);

	void GetResult(Span<byte> result);

	void Reset();
}


public static class IHashFunctionExtensions {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] GetResult(this IHashFunction chf) {
		var result = new byte[chf.DigestSize];
		chf.GetResult(result);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] Compute(this IHashFunction chf, ReadOnlySpan<byte> input) {
		chf.Transform(input);
		return chf.GetResult();
	}
}
