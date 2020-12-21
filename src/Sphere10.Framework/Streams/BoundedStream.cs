using System;
using System.IO;
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
			MinPosition = minPosition;
			MaxPosition = maxPosition;
		}

		public long MinPosition { get; }

		public long MaxPosition { get; }

		public override long Seek(long offset, SeekOrigin origin) {
			switch(origin) {
				case SeekOrigin.Begin:
					CheckPosition(offset);
					break;
				case SeekOrigin.Current:
					CheckPosition(Position + offset);
					break;
				case SeekOrigin.End:
					Guard.ArgumentInRange(offset, long.MinValue, 0, nameof(offset));
					CheckPosition(this.Length + offset);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
			}
			return base.Seek(offset, origin);
		}

		public override long Length {
			get {
				if (MaxPosition < MinPosition)
					return 0;

				var streamEndIX = InnerStream.Length - 1;
				if (streamEndIX < 0)
					return 0;

				if (MinPosition > streamEndIX)
					return 0;

				var actualEndPosition = MaxPosition <= streamEndIX ? MaxPosition : streamEndIX;

				return actualEndPosition - MinPosition + 1;
			}
		} 

		public override void SetLength(long value) {
			throw new NotSupportedException();
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
			var position = this.Position;
			CheckRange(position, Math.Max(position, position + count - 1));
		}

		protected void CheckRange(long start, long end) {
			Guard.Argument(end >= start, nameof(end), $"Must be greater than or equal to {nameof(start)}");
			if (start < MinPosition || end > MaxPosition)
				throw new StreamOutOfBoundsException();
		}
	}

}