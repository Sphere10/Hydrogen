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

public class WriteOnlyStream<TStream> : StreamDecorator<TStream> where TStream : Stream {
	private const string ErrorMessage = "Reading is not supported on this stream";

	public WriteOnlyStream(TStream stream)
		: base(stream) {
	}

	public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException(ErrorMessage);

	public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException(ErrorMessage);

	public override bool CanRead => false;

	public override bool CanSeek => false;

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => throw new NotSupportedException(ErrorMessage);

	public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => throw new NotSupportedException(ErrorMessage);

	public override int EndRead(IAsyncResult asyncResult) => throw new NotSupportedException(ErrorMessage);

	public override bool Equals(object obj) => throw new NotSupportedException(ErrorMessage);

	public override int ReadByte() => throw new NotSupportedException(ErrorMessage);

	public override int ReadTimeout => throw new NotSupportedException(ErrorMessage);

}

public class WriteOnlyStream : WriteOnlyStream<Stream> {
	public WriteOnlyStream(Stream stream)
		: base(stream) {
	}
}
