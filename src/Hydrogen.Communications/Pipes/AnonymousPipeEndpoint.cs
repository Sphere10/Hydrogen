using System;

namespace Sphere10.Framework.Communications {
	public class AnonymousPipeEndpoint {

		public AnonymousPipeEndpoint() : this(null, null) {
		}

		public AnonymousPipeEndpoint(string readHandle, string writeHandle) {
			ReaderHandle = readHandle;
			WriterHandle = writeHandle;
		}

		public string WriterHandle { get; init; }
		public string ReaderHandle { get; init; }

		public static AnonymousPipeEndpoint Empty => new() { ReaderHandle = string.Empty, WriterHandle = string.Empty };
	}
}
