// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A stream implementation that connects multiple disparate byte fragments into one logical stream. The fragments are managed through a <see cref="IStreamFragmentProvider"/>.
/// </summary>
public class FragmentedStream : Stream {

	private long _position;

	public FragmentedStream(IStreamFragmentProvider fragmentProvider)
		: this(fragmentProvider, fragmentProvider?.TotalBytes ?? 0) {
	}

	public FragmentedStream(IStreamFragmentProvider fragmentProvider, long streamLength) {
		Guard.ArgumentNotNull(fragmentProvider, nameof(fragmentProvider));
		Guard.ArgumentInRange(streamLength, 0, fragmentProvider.TotalBytes, nameof(streamLength));
		FragmentProvider = fragmentProvider;
		StreamLength = streamLength;
		Position = 0;
	}

	public IStreamFragmentProvider FragmentProvider { get; }

	public override bool CanRead => true;

	public override bool CanSeek => true;

	public override bool CanWrite => true;

	public override long Length => StreamLength;

	public long StreamLength { get; private set; }

	public override long Position {
		get => _position;
		set {
			Guard.ArgumentInRange(value, 0, Length, "Value");
			_position = value;
		}
	}

	public override void SetLength(long value) {
		Guard.ArgumentInRange(value, 0, int.MaxValue, nameof(value));
		FragmentProvider.SetTotalBytes(value);

		if (Position > value)
			Position = value;
		StreamLength = value;
	}

	public override long Seek(long offset, SeekOrigin origin) {
		switch (origin) {
			case SeekOrigin.Begin:
				Guard.ArgumentInRange(offset, 0, Math.Max(0, Length), nameof(offset));
				Position = offset;
				break;

			case SeekOrigin.Current:
				Guard.ArgumentInRange(offset, -Position, Math.Max(0, Length - Position), nameof(offset));
				Position += offset;
				break;

			case SeekOrigin.End:
				Guard.ArgumentInRange(offset, -Length, 0, nameof(offset));
				Position = Length + offset;
				break;
		}

		return Position;
	}

	public override int ReadByte() {
		if (Position >= Length)
			return -1;
		return base.ReadByte();
	}

	public override int Read(byte[] buffer, int offset, int count) {
		Guard.ArgumentNotNull(buffer, nameof(buffer));
		Guard.ArgumentInRange(offset, 0, Math.Max(0, buffer.Length - 1), nameof(offset));
		Guard.ArgumentInRange(count, 0, int.MaxValue, nameof(count));

		if (Length == 0)
			return 0;

		var remainingStreamBytes = Length - Position;
		var remainingBufferBytes = buffer.Length - offset;
		var bytesToRead = Math.Max(0, Math.Min(count, Math.Min(remainingBufferBytes, remainingStreamBytes)));

		var bufferSpan = buffer.AsSpan();
		var bufferOffset = offset;
		var totalRead = (int)bytesToRead;

		while (bytesToRead > 0) {
			FragmentProvider.MapStreamPosition(Position, out var fragmentIndex, out var fragmentPosition);
			var fragmentPositionI = Tools.Collection.CheckNotImplemented64bitAddressingLength(fragmentPosition);

			var fragment = FragmentProvider.GetFragment(fragmentIndex);

			var bytesToReadFromFragment = (int)Math.Min(fragment.Length - fragmentPositionI, bytesToRead);
			var fragmentSlice = fragment.Slice(fragmentPositionI, bytesToReadFromFragment);
			var bufferSlice = bufferSpan.Slice(bufferOffset, bytesToReadFromFragment);
			fragmentSlice.CopyTo(bufferSlice);

			bufferOffset += bytesToReadFromFragment;
			Position += bytesToReadFromFragment;
			bytesToRead -= bytesToReadFromFragment;
		}
		Debug.Assert(bytesToRead == 0);
		return totalRead;
	}

	public override void Write(byte[] buffer, int offset, int count) {
		Guard.ArgumentNotNull(buffer, nameof(buffer));
		if (count == 0)
			return;
		Guard.ArgumentInRange(offset, 0, buffer.Length - 1, nameof(offset));
		Guard.ArgumentInRange(count, 0, buffer.Length - offset, nameof(count));

		var remainingBufferBytes = buffer.Length - offset;
		var remainingStreamBytes = Length - Position;
		var bytesToWrite = (int)Math.Min(remainingBufferBytes, count);

		// Grow stream if required
		if (bytesToWrite > remainingStreamBytes) {
			var newStreamLength = Length + (bytesToWrite - remainingStreamBytes);
			var prePos = Position;
			FragmentProvider.SetTotalBytes(newStreamLength);
			
			StreamLength = newStreamLength;
			var postPos = Position;
			Guard.Ensure(postPos == prePos, "Stream pointer has moved during resize");
		}

		var bufferSpan = buffer.AsSpan();
		var bufferOffset = offset;
		var totalWrites = 0;
		while (bytesToWrite > 0) {
			FragmentProvider.MapStreamPosition(Position, out var fragmentIndex, out var fragmentPosition);
			var fragmentPositionI = Tools.Collection.CheckNotImplemented64bitAddressingLength(fragmentPosition);

			var fragment = FragmentProvider.GetFragment(fragmentIndex).ToArray();
			var bytesToWriteToFragment = Math.Min(fragment.Length - fragmentPosition, bytesToWrite);

			var bytesToWriteFramentI = Tools.Collection.CheckNotImplemented64bitAddressingLength(bytesToWriteToFragment);
			var fragmentSlice = fragment.AsSpan(fragmentPositionI, bytesToWriteFramentI);
			var bufferSlice = bufferSpan.Slice(bufferOffset, bytesToWriteFramentI);
			bufferSlice.CopyTo(fragmentSlice);
			FragmentProvider.UpdateFragment(fragmentIndex, fragmentPosition, fragmentSlice);

			bufferOffset += bytesToWriteFramentI;
			Position += bytesToWriteToFragment;
			totalWrites += bytesToWriteFramentI;
			bytesToWrite -= bytesToWriteFramentI;
		}
		Debug.Assert(bytesToWrite == 0);
	}

	public override void Flush() {
	}

	public virtual byte[] ToArray() {
		var builder = new ByteArrayBuilder();
		var position = 0L;
		var bytesToRead = Length;
		while (bytesToRead > 0) {
			FragmentProvider.MapStreamPosition(position, out var fragmentIndex, out var fragmentPosition);
			var fragmentPositionI = Tools.Collection.CheckNotImplemented64bitAddressingLength(fragmentPosition);

			var fragment = FragmentProvider.GetFragment(fragmentIndex);
			var bytesToReadFromFragment = (int)Math.Min(fragment.Length - fragmentPosition, bytesToRead);
			var fragmentBytes = fragment.Slice(fragmentPositionI, bytesToReadFromFragment);
			builder.Append(fragmentBytes);

			position += bytesToReadFromFragment;
			bytesToRead -= bytesToReadFromFragment;
		}
		Debug.Assert(bytesToRead == 0);
		return builder.ToArray();
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		if (disposing) {
			if (FragmentProvider is IDisposable disposable)
				disposable.Dispose();
		}
	}

	public override async ValueTask DisposeAsync() {
		await base.DisposeAsync();
		switch (FragmentProvider) {
			case IAsyncDisposable asyncDisposable:
				await asyncDisposable.DisposeAsync();
				break;
			case IDisposable disposable:
				disposable.Dispose();
				break;
		}
	}
}
