using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Sphere10.Framework {

	/// <summary>
	/// Ensures that all stream read/writes occur within a boundary of the stream. Also,
	/// ensures the length of the stream cannot be changed.
	/// </summary>
	public class BoundedStream : StreamDecorator {

		public BoundedStream(Stream innerStream, long minPosition, long maxPosition)
			: base(innerStream) {
			MinAbsolutePosition = minPosition;
			MaxAbsolutePosition = maxPosition;
			UseRelativeOffset = false;
			AllowResize = true;
		}

		public long MinAbsolutePosition { get; }

		public long MaxAbsolutePosition { get; }

		public bool AllowResize { get; set; }

		public override long Position {
			get => FromAbsoluteOffset(AbsolutePosition); 
			set => AbsolutePosition = ToAbsoluteOffset(value);
		}

		protected long AbsolutePosition {
			get => base.Position;
			set => base.Position = value;
		}

		/// <summary>
		/// When true, stream begins at <see cref="Position"/>=0, when false, begins at <see cref="Position"/>=<see cref="MinAbsolutePosition"/>. 
		/// </summary>
		public bool UseRelativeOffset { get; set; }

		public override long Seek(long offset, SeekOrigin origin) {
			switch (origin) {
				case SeekOrigin.Begin:
					CheckPosition(offset);
					break;
				case SeekOrigin.Current:
					CheckPosition(Position + offset);
					break;
				case SeekOrigin.End:
					Guard.ArgumentInRange(offset, long.MinValue, 0, nameof(offset));
					CheckPosition(Length == 0 ? 0 : Length -1 + offset);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
			}
			var absoluteOffset = ToAbsoluteOffset(offset);
			
			return base.Seek(absoluteOffset, origin);
		}


		public override long Length {
			get {
				if (MaxAbsolutePosition < MinAbsolutePosition)
					return 0;

				var streamEndIX = InnerStream.Length - 1;
				if (streamEndIX < 0)
					return 0;

				if (MinAbsolutePosition > streamEndIX)
					return 0;

				var actualEndPosition = MaxAbsolutePosition <= streamEndIX ? MaxAbsolutePosition : streamEndIX;

				return actualEndPosition - MinAbsolutePosition + 1;
			}
		} 

		public override void SetLength(long value) {
			if (AllowResize)
				InnerStream.SetLength(ToAbsoluteOffset(value));
			else throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count) {
			CheckRange(count);
			return base.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count) {
			CheckRange(count);
			InnerStream.Write(buffer, offset, count);
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
			CheckRange(count);
			return InnerStream.BeginRead(buffer, offset, count, callback, state);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
			CheckRange(count);
			return InnerStream.BeginWrite(buffer, offset, count, callback, state);
		}

		public override int ReadByte() {
			CheckCurrentPosition();
			return InnerStream.ReadByte();
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
			CheckRange(count);
			return InnerStream.WriteAsync(buffer, offset, count, cancellationToken);
		}

		public override void WriteByte(byte value) {
			CheckCurrentPosition();
			InnerStream.WriteByte(value);
		}

		protected void CheckCurrentPosition() {
			CheckPosition(Position);
		}

		protected void CheckPosition(long position) {
			CheckRange(position, position);	
		}

		protected void CheckRange(int count) {
			var position = Position;
			CheckRange(position, Math.Max(position, position + count - 1));
		}

		protected void CheckRange(long start, long end) {
			Guard.Argument(end >= start, nameof(end), $"Must be greater than or equal to {nameof(start)}");
			var absoluteStart = ToAbsoluteOffset(start);
			var absoluteEnd = ToAbsoluteOffset(end);
			if (absoluteStart < MinAbsolutePosition || absoluteEnd > MaxAbsolutePosition)
				throw new StreamOutOfBoundsException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private long ToAbsoluteOffset(long offset) => UseRelativeOffset ? offset + MinAbsolutePosition : offset;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private long FromAbsoluteOffset(long offset) => UseRelativeOffset ? offset - MinAbsolutePosition : offset;

	}

}