using Hydrogen;
using System;

namespace AbstractProtocol.AnonymousPipeComplex {
    [Serializable]
	public class RequestListFolder {
		public string Folder { get; init; }

		internal static RequestListFolder GenRandom() => new() {
			Folder = $"c:/SomeFolder/SomeSubFolder-{ Guid.NewGuid().ToStrictAlphaString() }"
		};
	}
}
