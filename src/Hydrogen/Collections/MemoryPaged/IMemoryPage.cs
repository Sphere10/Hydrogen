// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public interface IMemoryPage<TItem> : IPage<TItem>, IDisposable {

	/// <summary>
	/// The maximum byte size of the page.
	/// </summary>
	long MaxSize { get; set; }

	/// <summary>
	/// Saves the page to streams.
	/// </summary>
	void Save();

	/// <summary>
	/// Loads the page from streams.
	/// </summary>
	void Load();

	/// <summary>
	/// Unloads the page from memory.
	/// </summary>
	void Unload();

}
