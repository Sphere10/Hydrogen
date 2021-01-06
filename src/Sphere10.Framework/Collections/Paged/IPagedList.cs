using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sphere10.Framework {

    public interface IPagedList<TItem, TPage> : IExtendedList<TItem>
		where TPage : IPage<TItem> {

		event EventHandlerEx<object> Accessing;
		event EventHandlerEx<object> Accessed;
		event EventHandlerEx<object, TPage> PageAccessing;
		event EventHandlerEx<object, TPage> PageAccessed;
		event EventHandlerEx<object, int> PageCreating;
		event EventHandlerEx<object, TPage> PageCreated;
		event EventHandlerEx<object, TPage> PageReading;
		event EventHandlerEx<object, TPage> PageRead;
		event EventHandlerEx<object, TPage> PageWriting;
		event EventHandlerEx<object, TPage> PageWrite;
		event EventHandlerEx<object, TPage> PageDeleting;
		event EventHandlerEx<object, TPage> PageDeleted;

		IReadOnlyList<TPage> Pages { get; }

		bool RequiresLoad { get; }

		void Load();
	}
}