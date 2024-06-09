// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;

namespace Hydrogen;

public abstract class FileSwappedMemoryPage<TItem> : MemoryPageBase<TItem> {
	private readonly string _file;

	protected FileSwappedMemoryPage(long pageSize, IItemSizer<TItem> sizer, IExtendedList<TItem> memoryStore)
		: this(pageSize, Tools.FileSystem.GetTempFileName(false), sizer, memoryStore) {
	}

	protected FileSwappedMemoryPage(long pageSize, string fileStore, IItemSizer<TItem> sizer, IExtendedList<TItem> memoryStore)
		: base(pageSize, sizer, memoryStore) {
		_file = fileStore;
	}

	public override void Dispose() {
		if (File.Exists(_file))
			File.Delete(_file);
	}

	protected override Stream OpenReadStream() {
		if (!File.Exists(_file))
			return Stream.Null;
		return File.OpenRead(_file);
	}

	protected override Stream OpenWriteStream() {
		return File.Open(_file, FileMode.OpenOrCreate, FileAccess.Write);
	}

}
