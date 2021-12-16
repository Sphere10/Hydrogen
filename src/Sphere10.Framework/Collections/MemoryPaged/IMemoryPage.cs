using System;

namespace Sphere10.Framework {
	public interface IMemoryPage<TItem> : IPage<TItem>, IDisposable {

		/// <summary>
		/// The maximum byte size of the page.
		/// </summary>
		int MaxSize { get; set; }

		/// <summary>
		/// Saves the page to storage.
		/// </summary>
		void Save();
	
		/// <summary>
		/// Loads the page from storage.
		/// </summary>
		void Load();

		/// <summary>
		/// Unloads the page from memory.
		/// </summary>
		void Unload();

	}
}
