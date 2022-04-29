using System.IO;

namespace Hydrogen {

	public sealed class NonClosingStream : StreamDecorator {
		public NonClosingStream(Stream innerStream)
			: base(innerStream) {
		}

		public override void Close() {
			// do not close underlying stream
			// Note: overriding dispose is inconsequential
		}
	}
}