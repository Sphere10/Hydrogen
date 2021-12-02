using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// A stream implementation that connects multiple disparate byte fragments into one logical stream. The fragments are managed through a <see cref="IStreamFragmentProvider"/>.
	/// </summary>
	public class FragmentedStream : Stream {
		
		private readonly IStreamFragmentProvider _fragmentProvider;
		private long _position;

		public FragmentedStream(IStreamFragmentProvider fragmentProvider) {
			_fragmentProvider = fragmentProvider;
			_position = 0;
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
			var bufferIndex = offset;
			var remaining = (int)bytesToRead;

			// Starting with the current position's fragment, read bytes from fragment and move to the next until no further bytes remain
			var (fragmentIndex, fragmentPosition) = _fragmentProvider.MapLogicalPositionToFragment(Position, out var fragment);

			while (remaining > 0) {
				var fromFragmentCount = Math.Min(fragment.Length - fragmentPosition, remaining);
				var fragmentSlice = fragment.Slice(fragmentPosition, fromFragmentCount);

				var bufferSlice = bufferAsSpan.Slice(bufferIndex, fromFragmentCount);
				fragmentSlice.CopyTo(bufferSlice);

				bufferIndex += fromFragmentCount;
				remaining -= fromFragmentCount;

				if (remaining > 0) {
					fragmentIndex++;
					fragmentPosition = 0;
					fragment = _fragmentProvider.GetFragment(fragmentIndex);
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

		public override void SetLength(long value) {
			Guard.ArgumentInRange(value, 0, int.MaxValue, nameof(value));
			if (value < Length) {
				_fragmentProvider.ReleaseSpace((int)Length - (int)value, out _);
				if (_position > value) {
					_position = value;
				}
			} else if (value > Length) {
				if (!_fragmentProvider.TryRequestSpace((int)value - (int)Length, out _)) {
					throw new InvalidOperationException("Request for space from fragment provider was not successful.");
				}
			}
		}

		public override void Write(byte[] buffer, int offset, int count) {
			Guard.ArgumentNotNull(buffer, nameof(buffer));
			Guard.ArgumentInRange(offset, 0, buffer.Length - 1, nameof(offset));
			Guard.ArgumentInRange(count, 0, buffer.Length - offset, nameof(count));

			var updateAmount = (int)Math.Min(Length - Position, count);
			var addingAmount = Math.Max(0, count - updateAmount);
			Debug.Assert(updateAmount + addingAmount == count);

			//Update existing fragments, starting with the current position's fragment.
			var remaining = updateAmount;
			if (remaining > 0) {
				var (fragmentIndex, fragmentPosition) = _fragmentProvider.MapLogicalPositionToFragment(Position, out var fragment);
				var updatedBytes = new byte[remaining];
				Buffer.BlockCopy(buffer, offset, updatedBytes, 0, remaining);

				var updateIndex = 0;

				while (remaining > 0) {
					var sliceBytesCount = Math.Min(remaining, fragment.Length - fragmentPosition);

					_fragmentProvider.UpdateFragment(fragmentIndex, fragmentPosition, updatedBytes[updateIndex..(updateIndex + sliceBytesCount)]);
					updateIndex += sliceBytesCount;
					remaining -= sliceBytesCount;

					if (remaining > 0) {
						fragmentIndex++;
						fragment = _fragmentProvider.GetFragment(fragmentIndex);
						fragmentPosition = 0;
					}
				}

				Position += updateAmount;
			}

			// Request new space from the fragment provider, and write the new bytes.
			remaining = addingAmount;
			if (remaining > 0) {
				var addedBytes = new byte[remaining];
				Buffer.BlockCopy(buffer, offset + updateAmount, addedBytes, 0, remaining);

				var addIndex = 0;

				if (_fragmentProvider.TryRequestSpace(remaining, out var newFragmentIndexes)) {
					
					var (currentFragmentIndex, fragmentPosition) = _fragmentProvider.MapLogicalPositionToFragment(Position, out var fragment);
					// if the current fragment isn't new and has space available, fill it first.
					if (fragmentPosition != fragment.Length && !newFragmentIndexes.Contains(currentFragmentIndex)) {
						var currentFragmentRemainingBytes = fragment.Length - fragmentPosition;
						var unusedFragmentSlice = fragment.Slice(fragmentPosition, Math.Min(currentFragmentRemainingBytes, remaining));
						addedBytes.AsSpan(addIndex, unusedFragmentSlice.Length).CopyTo(unusedFragmentSlice);
						addIndex += unusedFragmentSlice.Length;
						remaining -= unusedFragmentSlice.Length;
					}
					
					if (remaining > 0) {
						foreach (var i in newFragmentIndexes) {
							var currentFragment = _fragmentProvider.GetFragment(i);
							var toAddAmount = Math.Min(currentFragment.Length, remaining);
							addedBytes[addIndex..(addIndex + toAddAmount)].CopyTo(currentFragment);
							_fragmentProvider.UpdateFragment(i, 0, currentFragment);
							
							addIndex += toAddAmount;
							remaining -= toAddAmount;
						}
					}

					Guard.Ensure(remaining == 0, "Internal Error");
					//Debug.Assert(remaining == 0);
					Position += addingAmount;
					
				} else {
					throw new InvalidOperationException("Request for space from fragment provider was not successful.");
				}
			}

			Debug.Assert(Position <= Length);
		}

		public override bool CanRead => true;

		public override bool CanSeek => true;

		public override bool CanWrite => true;

		public override long Length => _fragmentProvider.TotalBytes;

		public override long Position {
			get => _position;
			set {
				Guard.ArgumentInRange(value, 0, Length, "Value");
				_position = value;
			}
		}

		public virtual byte[] ToArray() {
			var builder = new ByteArrayBuilder();
			var remainingBytes = _fragmentProvider.TotalBytes;
			for (var i = 0; i < _fragmentProvider.FragmentCount; i++) {
				var fragment = _fragmentProvider.GetFragment(i);
				var takeAmount = (int)Math.Min(fragment.Length, remainingBytes);
				builder.Append(fragment.Slice(0, takeAmount));
				remainingBytes -= takeAmount;
			}
			return builder.ToArray();
		}
	}
}
