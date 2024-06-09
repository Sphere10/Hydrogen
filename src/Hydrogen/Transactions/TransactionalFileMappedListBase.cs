// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen;

public abstract class TransactionalFileMappedListBase<TItem> : FilePagedListBase<TItem>, ITransactionalFile {
	public event EventHandlerEx Committing;
	public event EventHandlerEx Committed;
	public event EventHandlerEx RollingBack;
	public event EventHandlerEx RolledBack;

	internal readonly MarkerRepository PageMarkerRepo;

	protected TransactionalFileMappedListBase(TransactionalFileDescriptor fileDescriptor, FileAccessMode accessMode = FileAccessMode.Default)
		: base(fileDescriptor, accessMode.WithoutAutoLoad()) {
		Guard.ArgumentNotNullOrEmpty(FileDescriptor.PagesDirectoryPath, nameof(FileDescriptor.PagesDirectoryPath));

		var pagesPath = FileDescriptor.PagesDirectoryPath;
		var ownsPagesPath = !string.IsNullOrWhiteSpace(FileDescriptor.PagesDirectoryPath) && !Path.IsPathFullyQualified(FileDescriptor.PagesDirectoryPath);
		if (ownsPagesPath) {
			pagesPath = Path.Combine(Tools.FileSystem.GetParentDirectoryPath(FileDescriptor.Path), pagesPath);
		} else if (!Directory.Exists(pagesPath)) {
			throw new DirectoryNotFoundException($"Directory not found: {pagesPath}");
		}
		FileID = ComputeFileID(FileDescriptor.CaseCorrectPath);
		PageMarkerRepo = new MarkerRepository(pagesPath, FileID, ownsPagesPath);
		// Empty files with uncommitted pages still require load (marked not by base constructor). 
		if (!RequiresLoad)
			RequiresLoad = File.Exists(FileDescriptor.Path);

		if (RequiresLoad && accessMode.HasFlag(FileAccessMode.AutoLoad)) {
			AccessMode |= FileAccessMode.AutoLoad;
			Load();
		}
	}

	public Guid FileID { get; }

	public new TransactionalFileDescriptor FileDescriptor => (TransactionalFileDescriptor)base.FileDescriptor;

	public abstract TransactionalFileMappedBuffer AsBuffer { get; }

	// TODO: base.Dirty O(N) -> O(1)
	public override bool Dirty => base.Dirty || PageMarkerRepo.PageMarkers.Any() || PageMarkerRepo.FileMarkers.Any();

	public void Commit() {
		CheckLoaded();
		CheckScopeInStatusIfExists(FileTransactionState.Committing);
		NotifyCommitting();
		Flush(); // flushing after notification ensures any mutations by handlers are captured
		PageMarkerRepo.Add(FileMarkerType.Committing);
		// All page files are manually copied
		Stream.SetLength(Count);
		foreach (var pageMarkers in PageMarkerRepo.PageMarkers) {
			var pageNumber = pageMarkers.Key;
			foreach (var marker in pageMarkers) {
				if (marker == PageMarkerType.UncommittedPage) {
					var pageFile = PageMarkerRepo.GetMarkerFilename(pageNumber, PageMarkerType.UncommittedPage);
					using var pageFileStream = File.OpenRead(pageFile);
					if (pageFileStream.Length <= 0)
						continue;
					var page = InternalPages[pageMarkers.Key];
					Stream.Seek(page.StartIndex, SeekOrigin.Begin);
					Tools.Streams.RouteStream(pageFileStream, Stream, pageFileStream.Length, blockSizeInBytes: HydrogenDefaults.TransactionalPageBufferOperationBlockSize);
				}
			}
		}
		Clear();
		PageMarkerRepo.RemoveAllPageMarkers();
		PageMarkerRepo.RemoveAllFileMarkers();
		RequiresLoad = true;
		Load();
		NotifyCommitted();
	}

	public Task CommitAsync() => Task.Run(Commit);

	public void Rollback() {
		CheckLoaded();
		CheckScopeInStatusIfExists(FileTransactionState.RollingBack);
		// Flush(); // 2024-03-11: removed since not needed theoretically
		NotifyRollingBack();
		PageMarkerRepo.Add(FileMarkerType.RollingBack);
		foreach (ITransactionalFilePage<TItem> page in InternalPages)
			page.HasUncommittedData = false;
		Clear();
		PageMarkerRepo.RemoveAllPageMarkers();
		PageMarkerRepo.RemoveAllFileMarkers();
		RequiresLoad = true;
		Load();
		NotifyRolledBack();
	}

	public Task RollbackAsync() => Task.Run(Rollback);

	public override void Dispose() {
		// Need to flush before disposing to ensure all data is written to disk and no page markers get left dangling
		if (FlushOnDispose) {
			Flush();
			FlushOnDispose = false;
		}

		if (!Disposing && !Disposed) {
			if (PageMarkerRepo.FileMarkers.Any() || PageMarkerRepo.PageMarkers.Any())
				Rollback();
		}

		PageMarkerRepo.Dispose();

		base.Dispose();
	}

	protected abstract long GetCommittedPageCount();

	protected override void OnLoaded() {
		base.OnLoaded();

		// Resume prior commit if applicable
		if (PageMarkerRepo.Contains(FileMarkerType.Committing))
			Commit();

		// Resume prior rollback if applicable
		if (PageMarkerRepo.Contains(FileMarkerType.RollingBack))
			Rollback();
	}

	protected override void OnPageCreating(long pageNumber) {
		base.OnPageCreating(pageNumber);
		if (IsLoading || Disposing)
			return;

		// If creating a new page on a previously deleted page, 
		// remove deleted marker and add empty page marker.
		// This ensures the new page loads as empty (otherwise will fill from original)
		if (PageMarkerRepo.Contains(PageMarkerType.DeletedMarker, pageNumber)) {
			PageMarkerRepo.Add(PageMarkerType.UncommittedPage, pageNumber);
			PageMarkerRepo.Remove(PageMarkerType.DeletedMarker, pageNumber);
		}
	}

	protected override void OnPageSaving(IMemoryPage<TItem> page) {
		base.OnPageSaving(page);
		if (Disposing)
			return;

		// Clear any deleted marker
		if (PageMarkerRepo.Contains(PageMarkerType.DeletedMarker, page.Number))
			PageMarkerRepo.Remove(PageMarkerType.DeletedMarker, page.Number);

		if (!PageMarkerRepo.Contains(PageMarkerType.UncommittedPage, page.Number)) {
			// Add the page marker
			PageMarkerRepo.Add(PageMarkerType.UncommittedPage, page.Number);
		}
	}

	protected override void OnPageSaved(IMemoryPage<TItem> page) {
		base.OnPageSaved(page);
		if (Disposing)
			return;

		// Truncate last page
		if (page.Number == InternalPages.Count - 1) {
			Tools.FileSystem.TruncateFile(PageMarkerRepo.GetMarkerFilename(page.Number, PageMarkerType.UncommittedPage), page.Size);
		}
	}

	protected override void OnPageDeleting(IPage<TItem> pageHeader) {
		base.OnPageDeleting(pageHeader);
		if (Disposing)
			return;

		// Create marker
		PageMarkerRepo.Remove(PageMarkerType.UncommittedPage, pageHeader.Number); // deletes uncommitted data
		var requiresDeletedMarker = pageHeader.Number < GetCommittedPageCount();
		if (requiresDeletedMarker)
			PageMarkerRepo.Add(PageMarkerType.DeletedMarker, pageHeader.Number);
	}

	protected virtual void OnCommitting() {
	}

	protected virtual void OnCommitted() {
	}

	protected virtual void OnRollingBack() {
	}

	protected virtual void OnRolledBack() {
	}

	private void NotifyCommitting() {
		OnCommitting();
		Committing?.Invoke();
	}

	private void NotifyCommitted() {
		OnCommitted();
		Committed?.Invoke();
	}

	private void NotifyRollingBack() {
		OnRollingBack();
		RollingBack?.Invoke();
	}

	private void NotifyRolledBack() {
		OnRolledBack();
		RolledBack?.Invoke();
	}

	public static Guid ComputeFileID(string caseCorrectFilePath) {
		// FileID is a first 16 bytes of the case-correct path converted into a guid
		Guard.ArgumentNotNull(caseCorrectFilePath, nameof(caseCorrectFilePath));
		return new Guid(Hashers.Hash(CHF.Blake2b_128, Encoding.UTF8.GetBytes(caseCorrectFilePath)).Take(16).ToArray());
	}

	private void CheckScopeInStatusIfExists(FileTransactionState status) {
		var txnScope = FileTransactionScope.GetCurrent();
		if (txnScope?.Transaction is null)
			return;

		// checks that if this is enlisted, that the scope triggered the commit
		if (FileTransaction.IsEnlisted(FileDescriptor.CaseCorrectPath) && txnScope.Transaction.Status != status) {
			throw new InvalidOperationException($"Commit or Rollback of a file enlisted in a {nameof(FileTransactionScope)} is prohibited. Call Commit/Rollback from the scope directly.");
		}
	}


	public enum PageMarkerType {
		UncommittedPage,
		DeletedMarker,
	}


	public enum FileMarkerType {
		Committing,
		RollingBack
	}


	/// <summary>
	/// Provides a persistable marker store based on files (can be resumed on abnormal termination)
	/// </summary>
	public class MarkerRepository : IDisposable {
		private readonly HashSet<FileMarkerType> _fileMarkers;
		private readonly ExtendedLookup<long, PageMarkerType> _pageMarkers;
		private readonly bool _maintainBaseDir;


		public MarkerRepository(string baseDir, Guid fileID, bool maintainBaseDir) {
			BaseDir = baseDir;
			FileID = fileID;
			_maintainBaseDir = maintainBaseDir;

			if (!Directory.Exists(baseDir)) 
				if (_maintainBaseDir)
					Directory.CreateDirectory(baseDir);
				else 
					throw new ArgumentException("Folder does not exist and is not being maintained by this marker repository", nameof(baseDir));

			ScanPersistedFileMarkers(baseDir, fileID, out _pageMarkers, out _fileMarkers);
		}

		public string BaseDir { get; }

		public Guid FileID { get; }

		public IEnumerable<FileMarkerType> FileMarkers => _fileMarkers;

		public ILookup<long, PageMarkerType> PageMarkers => _pageMarkers;

		public long? LowestDeletedPageNumber {
			get {
				var deletedMarkers = PageMarkers.Where(markers => markers.Contains(PageMarkerType.DeletedMarker)).Select(x => x.Key).ToArray();
				if (deletedMarkers.Length > 0)
					return deletedMarkers.Min();
				return null;
			}
		}

		public long? HighestChangedPageNumber {
			get {
				var pageMarkers = PageMarkers.Where(markers => markers.Contains(PageMarkerType.UncommittedPage)).Select(x => x.Key).ToArray();
				if (pageMarkers.Length > 0)
					return pageMarkers.Max();
				return null;
			}
		}

		public void Add(PageMarkerType marker, long pageNumber) {
			var addMarker = !_pageMarkers.Contains(pageNumber) || !_pageMarkers[pageNumber].Contains(marker);
			if (addMarker) {
				var file = GeneratePageMarkerFileName(BaseDir, FileID, marker, pageNumber);
				Tools.FileSystem.CreateBlankFile(file, _maintainBaseDir);
				_pageMarkers.Add(pageNumber, marker);
			}
		}

		public void Remove(PageMarkerType marker, long pageNumber) {
			var removeMarker = _pageMarkers.Contains(pageNumber) && _pageMarkers[pageNumber].Contains(marker);
			if (removeMarker) {
				var file = GeneratePageMarkerFileName(BaseDir, FileID, marker, pageNumber);
				File.Delete(file);
				_pageMarkers.Remove(pageNumber, marker);
			}
		}

		public void Remove(FileMarkerType marker) {
			var removeMarker = _fileMarkers.Contains(marker);
			if (removeMarker) {
				var file = GenerateFileMarkerFileName(BaseDir, FileID, marker);
				File.Delete(file);
				_fileMarkers.Remove(marker);
			}
		}

		public void Add(FileMarkerType marker) {
			var addMarker = !_fileMarkers.Contains(marker);
			if (addMarker) {
				var file = GenerateFileMarkerFileName(BaseDir, FileID, marker);
				Tools.FileSystem.CreateBlankFile(file, _maintainBaseDir);
				_fileMarkers.Add(marker);
			}
		}

		public void RemoveAllPageMarkersExcept(PageMarkerType except, long pageNumber) {
			RemoveAllPageMakers(pageNumber, except);
		}

		public void RemoveAllPageMakers(long pageNumber, params PageMarkerType[] except) {
			except = except ?? new PageMarkerType[0];
			foreach (var marker in Tools.Enums.GetValues<PageMarkerType>().Except(except)) {
				Remove(marker, pageNumber);
			}
		}

		public void RemoveAllPageMarkers() {
			var exceptions = new List<Exception>();
			foreach (var pageMarkers in _pageMarkers) {
				foreach (var marker in pageMarkers) {
					var path = GetMarkerFilename(pageMarkers.Key, marker);
					Tools.Exceptions.ExecuteCapturingException(() => File.Delete(path), exceptions);
				}
			}
			_pageMarkers.Clear();
			if (exceptions.Any())
				throw new AggregateException(exceptions);
		}

		public void RemoveAllFileMarkers() {
			var exceptions = new List<Exception>();
			foreach (var marker in _fileMarkers) {
				var path = GetMarkerFilename(marker);
				Tools.Exceptions.ExecuteCapturingException(() => File.Delete(path), exceptions);
			}
			_fileMarkers.Clear();
			if (exceptions.Any())
				throw new AggregateException(exceptions);
		}

		public bool Contains(FileMarkerType marker) {
			return _fileMarkers.Contains(marker);
		}

		public bool Contains(PageMarkerType marker, long pageNumber) {
			return _pageMarkers.Contains(pageNumber) && _pageMarkers[pageNumber].Contains(marker);
		}

		public string GetMarkerFilename(FileMarkerType marker) {
			return GenerateFileMarkerFileName(BaseDir, FileID, marker);
		}

		public string GetMarkerFilename(long pageNumber, PageMarkerType marker) {
			return GeneratePageMarkerFileName(BaseDir, FileID, marker, pageNumber);
		}

		public static void ScanPersistedFileMarkers(string baseDir, Guid fileID, out ExtendedLookup<long, PageMarkerType> pagerMarkers, out HashSet<FileMarkerType> fileMarkers) {
			pagerMarkers = new ExtendedLookup<long, PageMarkerType>();
			fileMarkers = new HashSet<FileMarkerType>();
			var filenameWithoutExtension = fileID.ToStrictAlphaString().ToLowerInvariant();
			var files = Tools.FileSystem.GetFiles(baseDir, $"{filenameWithoutExtension}.*");
			foreach (var file in files) {
				if (TryParseMarker(file, out var fileMarker, out var pageMarker, out var parsedFileID, out var pageNumber)) {
					// Other file marker
					if (parsedFileID != fileID)
						continue;
					if (fileMarker != null)
						fileMarkers.Add(fileMarker.Value);
					else if (pageMarker != null)
						pagerMarkers.Add(pageNumber.Value, pageMarker.Value);

				}
			}
		}

		public static bool TryParseMarker(string path, out FileMarkerType? fileMarker, out PageMarkerType? pageMarker, out Guid? fileID, out long? pageNumber) {
			var tokens = path.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
			fileMarker = null;
			pageMarker = null;
			fileID = null;
			pageNumber = null;
			string nakedFilePath;
			if (tokens.Length < 2)
				return false;
			var lastTokenIndex = tokens.Length - 1;
			var lastToken = tokens[lastTokenIndex];
			if (lastToken.IsIn("committing", "aborting")) {
				// file marker, no page number
				nakedFilePath = tokens.Take(tokens.Length - 1).ToDelimittedString(".");
				switch (lastToken) {
					case "committing":
						fileMarker = FileMarkerType.Committing;
						break;
					case "rollback":
						fileMarker = FileMarkerType.RollingBack;
						break;
					default:
						return false;
				}
			} else {
				// parse page number
				var pageNumTokenIndex = lastTokenIndex;
				switch (lastToken) {
					case "deleted":
						pageMarker = PageMarkerType.DeletedMarker;
						pageNumTokenIndex = lastTokenIndex - 1;
						break;
					default:
						pageMarker = PageMarkerType.UncommittedPage;
						break;
				}
				var numberToken = tokens[pageNumTokenIndex];
				if (!int.TryParse(numberToken, out var numberVal))
					return false;
				pageNumber = numberVal;
				nakedFilePath = tokens.Take(pageNumTokenIndex).ToDelimittedString(".");
			}
			var fileToken = System.IO.Path.GetFileName(nakedFilePath);
			if (!Guid.TryParse(fileToken, out var guidVal))
				return false;
			fileID = guidVal;
			return true;
		}

		public static string GeneratePageMarkerFileName(string baseDir, Guid fileID, PageMarkerType marker, long pageNumber) {
			string postfix;
			switch (marker) {
				case PageMarkerType.DeletedMarker:
					postfix = ".deleted";
					break;
				case PageMarkerType.UncommittedPage:
					postfix = string.Empty;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(marker), marker, null);
			}
			return System.IO.Path.Combine(baseDir, $"{fileID.ToStrictAlphaString().ToLowerInvariant()}.{pageNumber}{postfix}");
		}

		public static string GenerateFileMarkerFileName(string baseDir, Guid fileID, FileMarkerType marker) {
			string postfix;
			switch (marker) {
				case FileMarkerType.Committing:
					postfix = ".committing";
					break;
				case FileMarkerType.RollingBack:
					postfix = ".rollback";
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(marker), marker, null);
			}
			return System.IO.Path.Combine(baseDir, $"{fileID.ToStrictAlphaString().ToLowerInvariant()}{postfix}");
		}

		public void Dispose() {
			if (_maintainBaseDir && Tools.FileSystem.CountDirectoryContents(BaseDir) == 0)
				Directory.Delete(BaseDir, true);
		}
	}

}
