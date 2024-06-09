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

public class ReadOnlyStream<TStream> : StreamDecorator<TStream> where TStream : Stream {
	private const string ErrorMessage = "Writing is not supported on this stream";

	public ReadOnlyStream(TStream stream)
		: base(stream) {
	}
	
	public override void Flush() => throw new NotSupportedException(ErrorMessage);

	public override void SetLength(long value) => throw new NotSupportedException(ErrorMessage);

	public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException(ErrorMessage);

	public override bool CanWrite => false;
	
	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => throw new NotSupportedException(ErrorMessage);

	public override void EndWrite(IAsyncResult asyncResult) => throw new NotSupportedException(ErrorMessage);

	public override Task FlushAsync(CancellationToken cancellationToken) => throw new NotSupportedException(ErrorMessage);

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => throw new NotSupportedException(ErrorMessage);

	public override void WriteByte(byte value) => throw new NotSupportedException(ErrorMessage);

}

public class ReadOnlyStream : ReadOnlyStream<Stream> {
	public ReadOnlyStream(Stream stream)
		: base(stream) {
	}
}