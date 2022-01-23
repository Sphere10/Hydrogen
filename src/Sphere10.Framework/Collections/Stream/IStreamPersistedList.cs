using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IStreamPersistedList<TItem, out THeader, TRecord> : IExtendedList<TItem>
		where THeader : IStreamStorageHeader
		where TRecord : IStreamRecord {
		IStreamStorage<THeader, TRecord> Storage { get; }
	}


}
