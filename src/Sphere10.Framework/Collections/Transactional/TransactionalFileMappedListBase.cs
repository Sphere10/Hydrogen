//-----------------------------------------------------------------------
// <copyright file="PageManager.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sphere10.Framework {

	public abstract class TransactionalFileMappedListBase<TItem> : FilePagedListBase<TItem>,  ITransactionalFile {
		public event EventHandlerEx<object> Committing;
		public event EventHandlerEx<object> Committed;
		public event EventHandlerEx<object> RollingBack;
		public event EventHandlerEx<object> RolledBack;

		internal readonly MarkerRepository PageMarkerRepo;


		protected TransactionalFileMappedListBase(
			string filename, 
			string uncommittedPageFileDir, 
			int pageSize,
			long maxMemory,
			bool readOnly = false)
			: base(filename, pageSize, maxMemory, readOnly) { 
			Guard.ArgumentNotNullOrEmpty(uncommittedPageFileDir, nameof(uncommittedPageFileDir));
			if (!Directory.Exists(uncommittedPageFileDir))
				throw new DirectoryNotFoundException($"Directory not found: {uncommittedPageFileDir}");
			FileID = ComputeFileID(Path);
			PageMarkerRepo = new MarkerRepository(uncommittedPageFileDir, FileID);
		}

		public Guid FileID { get; }
		
		public abstract TransactionalFileMappedBuffer AsBuffer { get; }

		public override bool Dirty => base.Dirty || PageMarkerRepo.PageMarkers.Any() || PageMarkerRepo.FileMarkers.Any();

		public void Commit() {
			CheckScopeInStatusIfExists(FileTransactionState.Committing);
			Flush();
			NotifyCommitting();
			PageMarkerRepo.Add(FileMarkerType.Committing);
			// All page files are manually copied
			Stream.SetLength(this.Count);
			foreach (var pageMarkers in PageMarkerRepo.PageMarkers) {
				var pageNumber = pageMarkers.Key;
				foreach (var marker in pageMarkers) {
					if (marker == PageMarkerType.UncommittedPage) {
						var pageFile = PageMarkerRepo.GetMarkerFilename(pageNumber, PageMarkerType.UncommittedPage);
						var pageFileBytes = File.ReadAllBytes(pageFile);
						if (pageFileBytes.Length > 0) {
							var page = InternalPages[pageMarkers.Key];
							Stream.Seek(page.StartIndex, SeekOrigin.Begin);
							Stream.WriteBytes(pageFileBytes);
						}
					}
				}
			}
			Clear();
			PageMarkerRepo.RemoveAllPageMarkers();
			PageMarkerRepo.RemoveAllFileMarkers();
			Load();
			NotifyCommitted();
		}

		public void Rollback() {
			CheckScopeInStatusIfExists(FileTransactionState.RollingBack);
			NotifyRollingBack();
			Flush();
			PageMarkerRepo.Add(FileMarkerType.RollingBack);
			foreach (ITransactionalFilePage<TItem> page in InternalPages)
				page.HasUncommittedData = false;
			Clear();
			PageMarkerRepo.RemoveAllPageMarkers();
			PageMarkerRepo.RemoveAllFileMarkers();
			Load();
			NotifyRolledBack();
		}

		public override void Dispose() {
			if (PageMarkerRepo.FileMarkers.Any() || PageMarkerRepo.PageMarkers.Any())
				Rollback();
			base.Dispose();
		}

		public static Guid ComputeFileID(string caseCorrectFilePath) {
			// FileID is a first 16 bytes of the case-correct path converted into a guid
			Guard.ArgumentNotNull(caseCorrectFilePath, nameof(caseCorrectFilePath));
			return new Guid(Hashers.Hash(CHF.SHA2_256, Encoding.UTF8.GetBytes(caseCorrectFilePath)).Take(16).ToArray());
		}

		protected abstract int GetCommittedPageCount();

		protected override void OnLoaded() {
			base.OnLoaded();

			// Resume prior commit if applicable
			if (PageMarkerRepo.Contains(FileMarkerType.Committing))
				Commit();

			// Resume prior rollback if applicable
			if (PageMarkerRepo.Contains(FileMarkerType.RollingBack))
				Rollback();
		}

		protected override void OnPageCreating(int pageNumber) {
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
			Committing?.Invoke(this);
		}

		private void NotifyCommitted() {
			OnCommitted();
			Committed?.Invoke(this);
		}

		private void NotifyRollingBack() {
			OnRollingBack();
			RollingBack?.Invoke(this);
		}

		private void NotifyRolledBack() {
			OnRolledBack();
			RolledBack?.Invoke(this);
		}

		private void CheckScopeInStatusIfExists(FileTransactionState status) {
			var txnScope = FileTransactionScope.GetCurrent();
			if (txnScope?.Transaction is null)
				return;

			// checks that if this is enlisted, that the scope triggered the commit
			if (FileTransaction.IsEnlisted(Path) && txnScope.Transaction.Status != status) {
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
		public class MarkerRepository {
			readonly HashSet<FileMarkerType> _fileMarkers;
			readonly LookupEx<int, PageMarkerType> _pageMarkers;

			public MarkerRepository(string baseDir, Guid fileID) {
				BaseDir = baseDir;
				FileID = fileID;
				ScanPersistedFileMarkers(baseDir, fileID, out _pageMarkers, out _fileMarkers);
			}

			public string BaseDir { get; }

			public Guid FileID { get; }

			public IEnumerable<FileMarkerType> FileMarkers => _fileMarkers;

			public ILookup<int, PageMarkerType> PageMarkers => _pageMarkers;

			public int? LowestDeletedPageNumber {
				get {
					var deletedMarkers = PageMarkers.Where(markers => markers.Contains(PageMarkerType.DeletedMarker)).Select(x => x.Key).ToArray();
					if (deletedMarkers.Length > 0)
						return deletedMarkers.Min();
					return null;
				}
			}

			public int? HighestChangedPageNumber {
				get {
					var pageMarkers = PageMarkers.Where(markers => markers.Contains(PageMarkerType.UncommittedPage)).Select(x => x.Key).ToArray();
					if (pageMarkers.Length > 0)
						return pageMarkers.Max();
					return null;
				}
			}

			public void Add(PageMarkerType marker, int pageNumber) {
				var addMarker = !_pageMarkers.Contains(pageNumber) || !_pageMarkers[pageNumber].Contains(marker);
				if (addMarker) {
					var file = GeneratePageMarkerFileName(BaseDir, FileID, marker, pageNumber);
					Tools.FileSystem.CreateBlankFile(file);
					_pageMarkers.Add(pageNumber, marker);
				}
			}

			public void Remove(PageMarkerType marker, int pageNumber) {
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
					Tools.FileSystem.CreateBlankFile(file);
					_fileMarkers.Add(marker);
				}
			}

			public void RemoveAllPageMarkersExcept(PageMarkerType except, int pageNumber) {
				RemoveAllPageMakers(pageNumber, except);
			}

			public void RemoveAllPageMakers(int pageNumber, params PageMarkerType[] except) {
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
						Tools.Exceptions.ExecuteIgnoringException(() => File.Delete(path), exceptions);
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
					Tools.Exceptions.ExecuteIgnoringException(() => File.Delete(path), exceptions);
				}
				_fileMarkers.Clear();
				if (exceptions.Any())
					throw new AggregateException(exceptions);
			}

			public bool Contains(FileMarkerType marker) {
				return _fileMarkers.Contains(marker);
			}

			public bool Contains(PageMarkerType marker, int pageNumber) {
				return _pageMarkers.Contains(pageNumber) && _pageMarkers[pageNumber].Contains(marker);
			}

			public string GetMarkerFilename(FileMarkerType marker) {
				return GenerateFileMarkerFileName(BaseDir, FileID, marker);
			}

			public string GetMarkerFilename(int pageNumber, PageMarkerType marker) {
				return GeneratePageMarkerFileName(BaseDir, FileID, marker, pageNumber);
			}

			public static void ScanPersistedFileMarkers(string baseDir, Guid fileID, out LookupEx<int, PageMarkerType> pagerMarkers, out HashSet<FileMarkerType> fileMarkers) {
				pagerMarkers = new LookupEx<int, PageMarkerType>();
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

			public static bool TryParseMarker(string path, out FileMarkerType? fileMarker, out PageMarkerType? pageMarker, out Guid? fileID, out int? pageNumber) {
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

			public static string GeneratePageMarkerFileName(string baseDir, Guid fileID, PageMarkerType marker, int pageNumber) {
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

		}

	}

}