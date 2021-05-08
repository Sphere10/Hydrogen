using System;

namespace Sphere10.Framework {
	public interface IMemoryPage<TItem> : IPage<TItem>, IDisposable {
		int MaxSize { get; set; }

		void Save();
	
		void Load();

		void Unload();

	}
}
