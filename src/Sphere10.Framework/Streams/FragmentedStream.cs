using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Sphere10.Framework {
	public class FragmentedStream : Stream {
		// See ExtendedMemoryStream as an implementation guide
		private readonly IFragmentProvider _fragmentProvider;

		private long _position;
		private int _fragmentIndex;
		private int _fragmentPosition;

		public FragmentedStream(IEnumerable<byte[]> fragments)
			: this(new ByteArrayFragmentProvider(fragments)) {
		}

		public FragmentedStream(IFragmentProvider fragmentProvider) {
			_fragmentProvider = fragmentProvider;
			_position = 0;
			_fragmentIndex = 0;
		}

		public override void Flush() {
		}

		public override int Read(byte[] buffer, int offset, int count) {
			Guard.ArgumentNotNull(buffer, nameof(buffer));
			Guard.ArgumentInRange(offset, 0, buffer.Length - 1, nameof(offset));
			Guard.ArgumentInRange(count, 0, int.MaxValue, nameof(count));

			if (Length == 0)
				return 0;

			var remainingSourceBytes = Length - Position;
			var remainingBufferBytes = buffer.Length - offset;
			var bytesToRead = Math.Max(0, Math.Min(count, Math.Min(remainingBufferBytes, remainingSourceBytes)));

			var bufferAsSpan = buffer.AsSpan();
			int bufferIndex = offset;
			int remaining = (int)bytesToRead;

			while (remaining > 0) {
				Span<byte> fragment = _fragmentProvider.GetFragment(_fragmentIndex);
				int fromFragmentCount = Math.Min(fragment.Length - _fragmentPosition, remaining);
				Span<byte> fragmentSlice = fragment.Slice(_fragmentPosition, fromFragmentCount);

				Span<byte> bufferSlice = bufferAsSpan.Slice(bufferIndex, fromFragmentCount);
				fragmentSlice.CopyTo(bufferSlice);

				bufferIndex += fromFragmentCount;
				remaining -= fromFragmentCount;
				_fragmentPosition += fromFragmentCount;

				if (remaining > 0) {
					_fragmentIndex++;
					_fragmentPosition = 0;
				}
			}

			Position += bytesToRead;
			Debug.Assert(0 <= Position && Position <= Length);

			return (int)bytesToRead;
		}

		public override long Seek(long offset, SeekOrigin origin) {
			switch (origin) {
				case SeekOrigin.Begin:
					Guard.ArgumentInRange(offset, 0, Math.Max(0, Length), nameof(offset));
					SeekInternal(offset);
					break;

				case SeekOrigin.Current:
					Guard.ArgumentInRange(offset, -Position, Math.Max(0, Length - Position), nameof(offset));
					SeekInternal(_position + offset);
					break;

				case SeekOrigin.End:
					Guard.ArgumentInRange(offset, -Length, 0, nameof(offset));
					SeekInternal(Length + offset);
					break;
			}
			
			return _position;
		}

		public override void SetLength(long value) {
			Guard.ArgumentInRange(value, 0, int.MaxValue, nameof(value));
			if (value < Length) {
				_fragmentProvider.ReleaseSpace((int)Length - (int)value, out var released);
				if (_position > value)
					_position = value;
			} else if (value > Length) {
				if (!_fragmentProvider.TryRequestSpace((int)value - (int)Length, out var newFragmentIndexes)) {
					throw new InvalidOperationException("Request for space from fragment provider was not successful.");
				}
			}
		}

		public override void Write(byte[] buffer, int offset, int count) {
			Guard.ArgumentNotNull(buffer, nameof(buffer));
			Guard.ArgumentInRange(offset, 0, buffer.Length - 1, nameof(offset));
			Guard.ArgumentInRange(count, 0, buffer.Length - offset, nameof(count));

			var updateAmount = (int)Math.Min(Length - Position, count);
			var addingAmount = (int)Math.Max(0, count - updateAmount);
			Debug.Assert(updateAmount + addingAmount == count);

			int remaining = updateAmount;
			if (remaining > 0) {
				var updatedBytes = new byte[remaining];
				Buffer.BlockCopy(buffer, offset, updatedBytes, 0, remaining);

				int updateIndex = 0;

				while (remaining > 0) {
					var fragment = _fragmentProvider.GetFragment(_fragmentIndex);

					var updateSlice = fragment.Slice(_fragmentPosition);

					int sliceBytesCount = Math.Min(remaining, updateSlice.Length);

					updatedBytes[updateIndex..(updateIndex + sliceBytesCount)].CopyTo(updateSlice);
					updateIndex += sliceBytesCount;
					remaining -= sliceBytesCount;

					if (remaining > 0) {
						_fragmentIndex++;
						_fragmentPosition = 0;
					}
				}
			}

			remaining = addingAmount;
			if (remaining > 0) {
				var addedBytes = new byte[remaining];
				Buffer.BlockCopy(buffer, offset + updateAmount, addedBytes, 0, remaining);

				_fragmentIndex = _fragmentProvider.Count - 1;
				int addIndex = 0;

				if (_fragmentProvider.TryRequestSpace(remaining, out int[] spanIndexes)) {

					for (int i = 0; i < spanIndexes.Length; i++) {

						Span<byte> currentFragment = _fragmentProvider.GetFragment(spanIndexes[i]);
						_fragmentPosition = 0;

						int toAddAmount = Math.Min(currentFragment.Length, remaining);

						addedBytes[addIndex..(addIndex + toAddAmount)].CopyTo(currentFragment);
						addIndex += toAddAmount;
						remaining -= toAddAmount;

						_fragmentPosition += toAddAmount;
						_fragmentIndex++;
					}
				} else {
					throw new InvalidOperationException("Request for space from fragment provider was not successful.");
				}
			}

			_position += count;

			Debug.Assert(Position <= Length);
		}

		public override bool CanRead => true;

		public override bool CanSeek => true;

		public override bool CanWrite => true;

		public override long Length {
			get {
				int length = 0;
				for (int i = 0; i < _fragmentProvider.Count; i++) {
					length += _fragmentProvider.GetFragment(i).Length;
				}

				return length;
			}
		}

		public override long Position {
			get => _position;
			set {
				Guard.ArgumentInRange(value, 0, Length, "Value");
				_position = value;
			}
		}

		public virtual byte[] ToArray() {
			ByteArrayBuilder builder = new ByteArrayBuilder();
			for (int i = 0; i < _fragmentProvider.Count; i++) {
				builder.Append(_fragmentProvider.GetFragment(i));
			}
			return builder.ToArray();
		}


		private void SeekInternal(long newPosition) {

			if (newPosition == 0) {
				_fragmentIndex = 0;
				_fragmentPosition = 0;
				_position = newPosition;
				return;
			}
			
			long toMove = newPosition - _position;
			
			if (toMove > 0) {
				while (toMove > 0) {
					Span<byte> currentFragment = _fragmentProvider.GetFragment(_fragmentIndex);
					int fragmentMoveCount = (int)Math.Min(toMove, currentFragment.Length - _fragmentPosition);
					toMove -= fragmentMoveCount;
					_fragmentPosition += fragmentMoveCount;

					if (toMove > 0) {
						_fragmentIndex++;
						_fragmentPosition = 0;
					}
				}
			} else if (toMove < 0) {
				while (toMove < 0) {
					int fragmentMoveCount = (int)Math.Min(-toMove, _fragmentPosition);
					toMove += fragmentMoveCount;
					_fragmentPosition -= fragmentMoveCount;

					if (toMove < 0) {
						_fragmentIndex--;
						Span<byte> next = _fragmentProvider.GetFragment(_fragmentIndex);
						_fragmentPosition = next.Length;
					}
				}
			}

			_position = newPosition;
		}
	}
}
