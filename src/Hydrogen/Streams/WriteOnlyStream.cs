// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen {

	public abstract class WriteOnlyStream<TStream> : StreamDecorator<TStream> where TStream : Stream {
		private const string ErrorMessage = "Reading is not supported on this stream";

		protected WriteOnlyStream(TStream stream)
			: base(stream) {
		}

		public override void Flush() {
			InnerStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException(ErrorMessage);

		public override void SetLength(long value) {
			InnerStream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException(ErrorMessage);

		public override void Write(byte[] buffer, int offset, int count) {
			InnerStream.Write(buffer, offset, count);
		}

		public override bool CanRead => false;

		public override bool CanSeek => false;

		public override bool CanWrite => true;

		public override long Length => InnerStream.Length;

		public override long Position {
			get => InnerStream.Position;
			set => InnerStream.Position = value;
		}

		protected override void Dispose(bool disposing) {
			InnerStream.Dispose();
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => throw new NotSupportedException(ErrorMessage);

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
			return InnerStream.BeginWrite(buffer, offset, count, callback, state);
		}

		public override bool CanTimeout => InnerStream.CanTimeout;

		public override void Close() {
			base.Close();
			InnerStream.Close();
		}

		public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => throw new NotSupportedException(ErrorMessage);

		public override int EndRead(IAsyncResult asyncResult) => throw new NotSupportedException(ErrorMessage);

		public override void EndWrite(IAsyncResult asyncResult) {
			InnerStream.EndWrite(asyncResult);
		}

		public override bool Equals(object obj) => throw new NotSupportedException(ErrorMessage);

		public override Task FlushAsync(CancellationToken cancellationToken) {
			return InnerStream.FlushAsync(cancellationToken);
		}

		public override int GetHashCode() {
			return InnerStream.GetHashCode();
		}

		public override object InitializeLifetimeService() {
			return InnerStream.InitializeLifetimeService();
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
			return InnerStream.ReadAsync(buffer, offset, count, cancellationToken);
		}

		public override int ReadByte() => throw new NotSupportedException(ErrorMessage);

		public override int ReadTimeout => throw new NotSupportedException(ErrorMessage);

		public override string ToString() {
			return InnerStream.ToString();
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
			return InnerStream.WriteAsync(buffer, offset, count, cancellationToken);
		}

		public override void WriteByte(byte value) {
			InnerStream.WriteByte(value);
		}

		public override int WriteTimeout {
			get => InnerStream.WriteTimeout;
			set => InnerStream.WriteTimeout = value;
		}

		[Obsolete]
		protected override WaitHandle CreateWaitHandle() {
			throw new NotSupportedException();
		}

		[Obsolete]
		protected override void ObjectInvariant() {
			throw new NotSupportedException();
		}
	}

	public abstract class WriteOnlyStream : WriteOnlyStream<Stream>{
		protected WriteOnlyStream(Stream stream) 
			: base(stream) {
		}
	}
}
