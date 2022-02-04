using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sphere10.Framework {

	public class StreamPersistedListDecorator<TItem, THeader, TRecord, TConcrete> : SingularListDecorator<TItem, TConcrete>, IStreamPersistedList<TItem, THeader, TRecord>
		where THeader : IStreamStorageHeader
		where TRecord : IStreamRecord
		where TConcrete : IStreamPersistedList<TItem, THeader, TRecord> {
		
		public StreamPersistedListDecorator(TConcrete internalStreamPersistedList) : base(internalStreamPersistedList) {
		}

		public IStreamStorage<THeader, TRecord> Storage => InternalCollection.Storage;
	}

	public class StreamPersistedListDecorator<TItem, THeader, TRecord> : StreamPersistedListDecorator<TItem, THeader, TRecord, IStreamPersistedList<TItem, THeader, TRecord>>
		where THeader : IStreamStorageHeader
		where TRecord : IStreamRecord {

		public StreamPersistedListDecorator(IStreamPersistedList<TItem, THeader, TRecord> internalStreamPersistedList) : base(internalStreamPersistedList) {
		}
	}
}

