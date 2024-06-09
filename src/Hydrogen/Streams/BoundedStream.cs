// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// Wraps a stream and ensures that all read/writes occur within a specified boundary of a stream. Also addressing of bytes within the stream
/// can be made relative or absolute (see <see cref="UseRelativeAddressing"/>).
/// This is used to protect segments of streams within the family <see cref="IFilePagedList{TItem}" /> collections.
/// </summary>
/// <remarks>A <see cref="BoundedStream"/>'s boundary is not restricted by the length of the stream and can be defined beyond it.</remarks>
public class BoundedStream<TStream> : StreamDecorator<TStream> where TStream : Stream {

	public BoundedStream(TStream innerStream, long minPosition, long length)
		: base(innerStream) {
		Guard.ArgumentNotNull(innerStream, nameof(innerStream));
		Guard.ArgumentNotNegative(minPosition, nameof(minPosition));
		Guard.ArgumentNotNegative(length, nameof(length));
		MinAbsolutePosition = minPosition;
		MaxAbsolutePosition = checked(minPosition + length);
	}

	public long MinAbsolutePosition { get; }

	public long MaxAbsolutePosition { get; }

	public bool AllowInnerResize { get; set; } = true;

	public override long Position {
		get => FromAbsoluteAddress(AbsolutePosition);
		set => AbsolutePosition = ToAbsoluteAddress(value);
	}

	protected long AbsolutePosition {
		get => base.Position;
		set => base.Position = value;
	}

	/// <summary>
	/// When true, stream begins at <see cref="Position"/>=0, when false, begins at <see cref="Position"/>=<see cref="MinAbsolutePosition"/>. 
	/// </summary>
	public bool UseRelativeAddressing { get; set; } = false;

	public override long Seek(long offset, SeekOrigin origin) {
		var newAbsPosition = 0L;
		switch (origin) {
			case SeekOrigin.Begin:
				newAbsPosition = MinAbsolutePosition + offset;
				break;
			case SeekOrigin.Current:
				newAbsPosition = AbsolutePosition + offset;
				break;
			case SeekOrigin.End:
				newAbsPosition = MaxAbsolutePosition - offset;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
		}

		// NOTE: Seek is allowed to go beyond boundary (consistent with MemoryStream seek behaviour).

		var result = base.Seek(newAbsPosition, origin);
		return FromAbsoluteAddress(result);
	}


	public override long Length {
		get {
			var innerLength = InnerStream.Length;
			if (innerLength < MinAbsolutePosition)
				return 0;

			var streamEndIX = Math.Min(MaxAbsolutePosition, innerLength);
			var streamStartIX = Math.Max(0L, MinAbsolutePosition);
			return streamEndIX - streamStartIX;
		}
	}

	public long MaxLength => MaxAbsolutePosition - MinAbsolutePosition;

	public override void SetLength(long value) {
		if (AllowInnerResize)
			InnerStream.SetLength(MinAbsolutePosition + value);
		else throw new NotSupportedException();
	}

	public override int Read(byte[] buffer, int offset, int count) {
		// Special Case: when inner stream Position is at Tip just beyond current boundary and reading 0 bytes, 
		// this is valid
		if (count == 0)
			return 0;
		
		// ensure not reading from beyond boundary
		var absolutePosition = AbsolutePosition;
		CheckAbsolutePosition(absolutePosition);

		// Ensure read segment falls within boundary
		var overflow = absolutePosition + count - MaxAbsolutePosition;
		if (overflow > 0)
			count -= (int)overflow;

		return base.Read(buffer, offset, count);
	}

	public override void Write(byte[] buffer, int offset, int count) {
		CheckAbsoluteRange(AbsolutePosition, count);
		InnerStream.Write(buffer, offset, count);
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
		// Ensure not reading from beyond boundary
		var absolutePosition = AbsolutePosition;
		CheckAbsolutePosition(absolutePosition);

		// Ensure read segment falls within boundary
		var overflow = absolutePosition + count - MaxAbsolutePosition;
		if (overflow > 0)
			count -= (int)overflow;

		return InnerStream.BeginRead(buffer, offset, count, callback, state);
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
		var absolutePosition = AbsolutePosition;
		CheckAbsoluteRange(absolutePosition, count);
		return InnerStream.BeginWrite(buffer, offset, count, callback, state);
	}

	public override int ReadByte() {
		CheckAbsoluteRange(AbsolutePosition, 1);
		return InnerStream.ReadByte();
	}

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
		CheckAbsoluteRange(AbsolutePosition, count);
		return InnerStream.WriteAsync(buffer, offset, count, cancellationToken);
	}

	public override void WriteByte(byte value) {
		CheckAbsoluteRange(AbsolutePosition, 1);
		InnerStream.WriteByte(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private long ToAbsoluteAddress(long position) => UseRelativeAddressing ? position + MinAbsolutePosition : position;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private long FromAbsoluteAddress(long position) => UseRelativeAddressing ? position - MinAbsolutePosition : position;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckAbsolutePosition(long absolutePos) {
		if (absolutePos < MinAbsolutePosition || absolutePos > MaxAbsolutePosition)
			throw new StreamOutOfBoundsException("Operation would result in position outside of stream boundary");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckAbsoluteRange(long absoluteFrom, long count) {
		if (absoluteFrom < MinAbsolutePosition || absoluteFrom > MaxAbsolutePosition)
			throw new StreamOutOfBoundsException("Operation would result in position outside of stream boundary");

		var endIX = absoluteFrom + count;
		if (endIX < MinAbsolutePosition || endIX > MaxAbsolutePosition)
			throw new StreamOutOfBoundsException("Operation would result in position outside of stream boundary");
	}

}

public class BoundedStream : BoundedStream<Stream>{

	public BoundedStream(Stream innerStream, long minPosition, long length)
		: base(innerStream, minPosition, length) {
	}

}
