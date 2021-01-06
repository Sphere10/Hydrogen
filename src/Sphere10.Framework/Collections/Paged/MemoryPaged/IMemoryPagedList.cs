using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sphere10.Framework {

    public interface IMemoryPagedList<TItem, TPage> : IPagedList<TItem, TPage>
		where TPage : IPage<TItem> {

		event EventHandlerEx<object, TPage> PageLoading;
		event EventHandlerEx<object, TPage> PageLoaded;
		event EventHandlerEx<object, TPage> PageSaving;
		event EventHandlerEx<object, TPage> PageSaved;
		event EventHandlerEx<object, TPage> PageUnloading;
		event EventHandlerEx<object, TPage> PageUnloaded;
	}
}