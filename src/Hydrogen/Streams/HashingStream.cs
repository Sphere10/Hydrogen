// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen;

public sealed class HashingStream : WriteOnlyStream {
	private readonly IHashFunction _hashFunction;
	private readonly Disposables _disposables;
	private bool _canWrite;

	public HashingStream(CHF hashFunction)
		: base(new MemoryStream()) {
		_disposables = new Disposables();
		_disposables.Add(Hashers.BorrowHasher(hashFunction, out _hashFunction));
		_canWrite = true;
	}

	public byte[] GetDigest() {
		_canWrite = false;
		return _hashFunction.GetResult();
	}

	public override void Write(byte[] buffer, int offset, int count) {
		CheckCanWrite();
		_hashFunction.Transform(buffer.AsSpan(offset, count));
	}

	public override void Write(ReadOnlySpan<byte> buffer) {
		CheckCanWrite();
		_hashFunction.Transform(buffer);
	}

	public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
		CheckCanWrite();
		_hashFunction.Transform(buffer.AsSpan(offset, count));
	}

	public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = new CancellationToken()) {
		CheckCanWrite();
		_hashFunction.Transform(buffer.Span);
	}

	public override void WriteByte(byte value) {
		CheckCanWrite();
		_hashFunction.Transform(new byte[] { value });
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		_disposables.Dispose();
	}

	private void CheckCanWrite() {
		if (!_canWrite)
			throw new InvalidOperationException("Hashing stream cannot get written to after digest has been computed");
	}
}
