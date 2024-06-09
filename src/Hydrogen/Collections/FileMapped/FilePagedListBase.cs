// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// A list whose items are mapped onto pages of a file and file pages are cached in memory.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public abstract class FilePagedListBase<TItem> : MemoryPagedListBase<TItem>, IFilePagedList<TItem> {

	protected FilePagedListBase(PagedFileDescriptor pagedFileDescriptor, FileAccessMode accessMode = FileAccessMode.Default)
		: base(pagedFileDescriptor.PageSize, pagedFileDescriptor.MaxMemory, autoLoad: false) {
		FileDescriptor = pagedFileDescriptor;
		AccessMode = accessMode;
		Stream = FileAccessHelper.Open(pagedFileDescriptor, accessMode, out var requiresLoad, out var flushOnDispose);
		RequiresLoad = requiresLoad;
		FlushOnDispose = flushOnDispose;
		if (RequiresLoad && accessMode.HasFlag(FileAccessMode.AutoLoad)) {
			AccessMode |= FileAccessMode.AutoLoad;
			Load();
		}
	}
	
	public FileAccessMode AccessMode { get; protected set; }
	
	public PagedFileDescriptor FileDescriptor { get; }

	public override bool IsReadOnly => AccessMode.IsReadOnly();

	internal FileStream Stream { get; }

	public override long Count {
		get {
			CheckLoaded();
			return base.Count;
		}
	}

	public override void Flush() {
		base.Flush();
		if (!IsReadOnly)
			Stream.Flush(true);
	}

	public override void Dispose() {
		base.Dispose();
		Stream.Dispose();
	}

	// Force sub-class to implement
	protected abstract override IPage<TItem>[] LoadPages();

	protected override void OnPageCreated(IPage<TItem> page) {
		base.OnPageCreated(page);
		var filePage = (IFilePage<TItem>)page;
		if (filePage.Number == 0) {
			filePage.StartPosition = 0;
			filePage.EndPosition = -1;
		} else {
			var lastPage = (IFilePage<TItem>)InternalPages[filePage.Number - 1];
			filePage.StartPosition = lastPage.EndPosition + 1;
			filePage.EndPosition = lastPage.EndPosition;
		}
	}

	protected virtual void TruncateFile() {
		// This ensures file is exact size of it's content, excluding any pre-allocated append buffer
		var pages = InternalPages;
		var streamLength = pages.Count > 0 ? ((IFilePage<TItem>)pages.Last()).EndPosition + 1 : 0;
		if (Stream.Length != streamLength) {
			Stream.SetLength(streamLength);
		}
	}

}