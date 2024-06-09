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
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A stream that writes to an underlying <see cref="IBuffer"/> rather than a byte array (as <see cref="MemoryStream"/> does).
/// The purpose of this class is to provide a Stream similar in principle to a  <see cref="MemoryStream"/> but whose underlying streams of bytes
/// is decoupled from "contiguous block of memory". Example use cases include the ability to construct an arbitrarily large memory stream (by passing in an <see cref="MemoryPagedBuffer"/>) whose
/// contents are file-swapped automatically. The ability to construct a logical memory stream from a multitude of byte-fragments (via <see cref="FragmentedStream"/>).
/// </summary>
/// <remarks><see cref="ExtendedMemoryStream"/>'s are used extensively throughout the Hydrogen Framework in particular as an internal mechanism to facilitate
/// memory paged, clustered, stream mapped and merkleized collections.
/// </remarks>
public class ExtendedMemoryStream : Stream, ILoadable {
	public event EventHandlerEx<object> Loading;
	public event EventHandlerEx<object> Loaded;

	private long _position;
	private readonly bool _disposeSource;
	private readonly IBuffer _source;

	public ExtendedMemoryStream()
		: this(new MemoryBuffer()) {
	}

	public ExtendedMemoryStream(IBuffer source, bool disposeSource = false) {
		Guard.ArgumentNotNull(source, nameof(source));
		_source = source;
		_position = 0;
		_disposeSource = disposeSource;
	}

	public bool RequiresLoad => _source is ILoadable { RequiresLoad: true };

	public void Load() {
		if (_source is ILoadable { RequiresLoad: true } loadable) {
			Loading?.Invoke(this);
			loadable.Load();
			Loaded?.Invoke(this);
		}
	}

	public async Task LoadAsync() {
		if (_source is ILoadable { RequiresLoad: true } loadable) {
			Loading?.Invoke(this);
			await loadable.LoadAsync();
			Loaded?.Invoke(this);
		}
	}

	public override bool CanRead => true;

	public override bool CanSeek => true;

	public override bool CanWrite => true;

	public override void Flush() {
		if (_source is IMemoryPagedList<byte> memPagedList) // TODO: add Flushable abstraction?
			memPagedList.Flush();
	}

	public override long Length => RequiresLoad ? throw new InvalidOperationException("Stream is not loaded") : _source.Count;

	public override long Position {
		get => _position;
		set {
			Guard.ArgumentInRange(value, 0, Length, "Value");
			_position = value;
		}
	}

	public override int Read(byte[] buffer, int offset, int count) {
		Guard.ArgumentNotNull(buffer, nameof(buffer));
		Guard.ArgumentInRange(offset, 0, buffer.Length - 1, nameof(offset));
		Guard.ArgumentInRange(count, 0, int.MaxValue, nameof(count));

		if (Length == 0)
			return 0;

		var remainingSourceBytes = _source.Count - Position;
		var remainingBufferBytes = buffer.Length - offset;
		var bytesRead = Math.Max(0, Math.Min(count, Math.Min(remainingBufferBytes, remainingSourceBytes)));

		var bytes = _source.ReadSpan((int)Position, (int)bytesRead);
		bytes.CopyTo(buffer.AsSpan(offset, (int)bytesRead));

		Position += bytesRead;
		Debug.Assert(0 <= Position && Position <= Length);
		return (int)bytesRead;
	}

	/// <summary>
	/// Set up the stream position
	/// </summary>
	/// <param name="offset">Position</param>
	/// <param name="origin">Position origin</param>
	/// <returns>Position after setup</returns>
	public override long Seek(long offset, SeekOrigin origin) {
		switch (origin) {
			case (SeekOrigin.Begin):
				Guard.ArgumentInRange(offset, 0, Math.Max(0, Length), nameof(offset));
				Position = offset;
				break;

			case (SeekOrigin.Current):
				Guard.ArgumentInRange(offset, -Position, Math.Max(0, Length - Position), nameof(offset));
				Position = Position + offset;
				break;

			case (SeekOrigin.End):
				Guard.ArgumentInRange(offset, -Length, 0, nameof(offset));
				Position = Length + offset;
				break;

		}
		return Position;
	}

	public override void SetLength(long value) {
		Guard.ArgumentInRange(value, 0, int.MaxValue, nameof(value));
		if (value < Length) {
			_source.RemoveRange((int)value, (int)(Length - value));
			if (Position > value)
				Position = value;
		} else if (value > Length) {
			_source.AddRange(Tools.Array.Gen((int)(value - Length), (byte)0).AsSpan());
		}
	}

	public override void Write(byte[] buffer, int offset, int count) {
		Guard.ArgumentNotNull(buffer, nameof(buffer));
		Guard.ArgumentInRange(offset, 0, buffer.Length - 1, nameof(offset));
		Guard.ArgumentInRange(count, 0, buffer.Length - offset, nameof(count));

		var updateAmount = (int)Math.Min(Length - Position, count);
		var addingAmount = (int)Math.Max(0, count - updateAmount);
		Debug.Assert(updateAmount + addingAmount == count);

		if (updateAmount > 0) // Optimize bellow allocation (10mb clusters will allocate 10mb array)
			_source.UpdateRange((int)Position, buffer.AsSpan(offset, updateAmount));

		if (addingAmount > 0)
			_source.AddRange(buffer.AsSpan(offset + updateAmount, addingAmount));

		Position += updateAmount + addingAmount;
		Debug.Assert(Position <= Length);
	}

	public virtual byte[] ToArray() {
		return _source.ToArray();
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		if (disposing && _disposeSource && _source is IDisposable disposable)
			disposable.Dispose();
	}

}
