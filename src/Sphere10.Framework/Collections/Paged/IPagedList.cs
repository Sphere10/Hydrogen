using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sphere10.Framework {

    public interface IPagedList<TItem> : IExtendedList<TItem>, ILoadable {

		event EventHandlerEx<object> Accessing;
		event EventHandlerEx<object> Accessed;
		public event EventHandlerEx<object> Loading;
		public event EventHandlerEx<object> Loaded;
		event EventHandlerEx<object, IPage<TItem>> PageAccessing;
		event EventHandlerEx<object, IPage<TItem>> PageAccessed;
		event EventHandlerEx<object, int> PageCreating;
		event EventHandlerEx<object, IPage<TItem>> PageCreated;
		event EventHandlerEx<object, IPage<TItem>> PageReading;
		event EventHandlerEx<object, IPage<TItem>> PageRead;
		event EventHandlerEx<object, IPage<TItem>> PageWriting;
		event EventHandlerEx<object, IPage<TItem>> PageWrite;
		event EventHandlerEx<object, IPage<TItem>> PageDeleting;
		event EventHandlerEx<object, IPage<TItem>> PageDeleted;

		IDisposable EnterOpenPageScope(IPage<TItem> page);

		internal IReadOnlyList<IPage<TItem>> Pages { get; }

	}
}