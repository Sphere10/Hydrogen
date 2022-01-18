using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IStreamStorage<out THeader, out TRecord> : IStreamStorage
		where THeader : IStreamStorageHeader
		where TRecord : IStreamRecord {

		THeader Header { get; }

		IReadOnlyList<TRecord> Records { get; }

	}

}
