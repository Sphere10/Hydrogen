using System.IO;

namespace Sphere10.Framework {

	public class BoundedStreamProvider : IStreamProvider {

		public BoundedStreamProvider(Stream sourceStream, long startPostion, long endPosition, bool ownsStream = false) {
			OwnsStream = ownsStream;
			Stream =  new BoundedStream(sourceStream, startPostion, endPosition);
		}

		protected BoundedStream Stream { get; }

		protected bool OwnsStream { get; }

		public virtual Stream OpenReadStream() {
			Stream.Seek(Stream.MinAbsolutePosition, SeekOrigin.Begin);
			return new NonClosingStream( Stream );
		}

		public virtual Stream OpenWriteStream() {
			Stream.Seek(Stream.MinAbsolutePosition, SeekOrigin.Begin);
			return new NonClosingStream( Stream );
		}

		public virtual void Dispose() {
			if (OwnsStream)
				Stream.Dispose();
		}
	}

}