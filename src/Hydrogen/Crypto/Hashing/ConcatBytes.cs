// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// A mock hash algorithm used for primarily for testing merkle-trees.
/// </summary>
internal class ConcatBytes : HashFunctionBase {
	private readonly ByteArrayBuilder _builder;

	public ConcatBytes() {
		_builder = new ByteArrayBuilder();
	}

	public override int DigestSize => _builder.Length;

	public override void Initialize() {
		base.Initialize();
		_builder.Clear();
	}

	public override void Transform(ReadOnlySpan<byte> data) {
		base.Transform(data);
		_builder.Append(data.ToArray());
	}

	protected override void Finalize(Span<byte> digest) {
		_builder.ToArray().AsSpan().CopyTo(digest);
	}

	public override object Clone() {
		throw new NotImplementedException();
	}
}
