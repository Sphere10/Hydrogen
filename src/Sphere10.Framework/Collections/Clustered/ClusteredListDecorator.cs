using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sphere10.Framework {

	public class ClusteredListDecorator<TItem, THeader, TRecord, TConcrete> : StreamPersistedListDecorator<TItem, THeader, TRecord, TConcrete>, IClusteredList<TItem, THeader, TRecord>
		where THeader : IClusteredStorageHeader
		where TRecord : IClusteredRecord 
		where TConcrete : IClusteredList<TItem, THeader, TRecord> { 

		public ClusteredListDecorator(TConcrete internalClusteredList) 
			: base(internalClusteredList) {
		}
	}

	public class ClusteredListDecorator<TItem, THeader, TRecord> : StreamPersistedListDecorator<TItem, THeader, TRecord, IClusteredList<TItem, THeader, TRecord>>
		where THeader : IClusteredStorageHeader
		where TRecord : IClusteredRecord {
		public ClusteredListDecorator(IClusteredList<TItem, THeader, TRecord> internalClusteredList) 
			: base(internalClusteredList) {
		}
	}

}

