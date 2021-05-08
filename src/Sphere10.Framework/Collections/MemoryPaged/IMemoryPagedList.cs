using System;

namespace Sphere10.Framework {

	public interface IMemoryPagedList<TItem> : IPagedList<TItem>, IDisposable {

		event EventHandlerEx<object, IMemoryPage<TItem>> PageLoading;
		event EventHandlerEx<object, IMemoryPage<TItem>> PageLoaded;
		event EventHandlerEx<object, IMemoryPage<TItem>> PageSaving;
		event EventHandlerEx<object, IMemoryPage<TItem>> PageSaved;
		event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloading;
		event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloaded;

		void Flush();
	}
}