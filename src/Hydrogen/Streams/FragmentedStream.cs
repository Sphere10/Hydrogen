// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using System.IO;

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
		if (!FragmentProvider.TrySetTotalBytes(value, out _, out _))
			throw new InvalidOperationException($"Fragment provider unable to accomodate new length request of {value} bytes");

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

	public override int Read(byte[] buffer, int offset, int count) {
		Guard.ArgumentNotNull(buffer, nameof(buffer));
		Guard.ArgumentInRange(offset, 0, buffer.Length - 1, nameof(offset));
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
			if (!FragmentProvider.TryMapStreamPosition(Position, out var fragmentIndex, out var fragmentPosition))
				throw new InvalidOperationException($"Unable to resolve fragment for stream position {Position}");
			var fragment = FragmentProvider.GetFragment(fragmentIndex);

			var bytesToReadFromFragment = (int)Math.Min(fragment.Length - fragmentPosition, bytesToRead);
			var fragmentSlice = fragment.Slice(fragmentPosition, bytesToReadFromFragment);
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
			if (!FragmentProvider.TrySetTotalBytes(newStreamLength, out _, out _))
				throw new InvalidOperationException($"Unable to grow stream to length {newStreamLength}");
			StreamLength = newStreamLength;
		}

		var bufferSpan = buffer.AsSpan();
		var bufferOffset = offset;
		var totalWrites = 0;
		while (bytesToWrite > 0) {
			if (!FragmentProvider.TryMapStreamPosition(Position, out var fragmentIndex, out var fragmentPosition))
				throw new InvalidOperationException($"Unable to resolve fragment for stream position {Position}");
			var fragment = FragmentProvider.GetFragment(fragmentIndex).ToArray();

			var bytesToWriteToFragment = Math.Min(fragment.Length - fragmentPosition, (int)bytesToWrite);
			var fragmentSlice = fragment.AsSpan(fragmentPosition, bytesToWriteToFragment);
			var bufferSlice = bufferSpan.Slice(bufferOffset, bytesToWriteToFragment);
			bufferSlice.CopyTo(fragmentSlice);
			FragmentProvider.UpdateFragment(fragmentIndex, fragmentPosition, fragmentSlice);

			bufferOffset += bytesToWriteToFragment;
			Position += bytesToWriteToFragment;
			totalWrites += bytesToWriteToFragment;
			bytesToWrite -= bytesToWriteToFragment;
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
			if (!FragmentProvider.TryMapStreamPosition(position, out var fragmentIndex, out var fragmentPosition))
				throw new InvalidOperationException($"Unable to resolve fragment for stream position {position}");

			var fragment = FragmentProvider.GetFragment(fragmentIndex);
			var bytesToReadFromFragment = (int)Math.Min(fragment.Length - fragmentPosition, bytesToRead);
			var fragmentBytes = fragment.Slice(fragmentPosition, bytesToReadFromFragment);
			builder.Append(fragmentBytes);

			position += bytesToReadFromFragment;
			bytesToRead -= bytesToReadFromFragment;
		}
		Debug.Assert(bytesToRead == 0);
		return builder.ToArray();
	}
}
