// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Diagnostics;
using System.IO;

namespace Hydrogen;

public abstract class TransactionalFilePageBase<TItem> : FilePageBase<TItem>, ITransactionalFilePage<TItem> {

	private const int StreamCopyDefaultBlockSize = 262144; // 256k read blocks
	protected TransactionalFilePageBase(FileStream sourceFile, IItemSizer<TItem> sizer, string uncommittedPageFileName, long pageNumber, long pageSize, IExtendedList<TItem> memoryStore)
		: base(sourceFile, sizer, pageNumber, pageSize, memoryStore) {
		UncommittedPageFileName = uncommittedPageFileName;
		HasUncommittedData = File.Exists(UncommittedPageFileName);
	}

	public string UncommittedPageFileName { get; }

	public bool HasUncommittedData { get; set; }

	public Stream OpenSourceReadStream() {
		return base.OpenReadStream();
	}

	public Stream OpenSourceWriteStream() {
		return base.OpenWriteStream();
	}

	private void CreateUncommittedStream() {
		Debug.Assert(File.Exists(UncommittedPageFileName), "Uncommitted page marker not created");
		// create file marker

		// Write source data into the uncommitted page marker.
		// This marker is also the store for Uncommitted data.
		// When created, it contains the original source data.
		using (var readStream = OpenSourceReadStream())
		using (var uncommittedPageStream = File.Open(UncommittedPageFileName, FileMode.Truncate, FileAccess.Write))
			Tools.Streams.RouteStream(readStream, uncommittedPageStream, readStream.Length, blockSizeInBytes: StreamCopyDefaultBlockSize);

		HasUncommittedData = true;
	}

	protected override Stream OpenReadStream() {
		return HasUncommittedData ? File.OpenRead(UncommittedPageFileName) : base.OpenReadStream();
	}

	protected override Stream OpenWriteStream() {
		if (!HasUncommittedData)
			CreateUncommittedStream();
		Debug.Assert(HasUncommittedData);
		return File.OpenWrite(UncommittedPageFileName);
	}
}
