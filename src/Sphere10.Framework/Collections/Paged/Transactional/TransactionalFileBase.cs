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
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	public abstract class TransactionalFileBase<TItem, TPage> : PagedFileBase<TItem, TPage>, ITransactionalFile
		where TPage : TransactionalFileBase<TItem, TPage>.TransactionalPageBase {

		internal readonly MarkerRepository PageMarkerRepo;

		protected TransactionalFileBase(
			string filename, 
			string uncomittedPageFileDir, 
			Guid fileID,
			int pageSize,
			int maxCacheCapacity,
			CacheCapacityPolicy cachePolicy,
			bool readOnly = false)
			: base(filename, pageSize, maxCacheCapacity, cachePolicy, readOnly) { 
			Guard.ArgumentNotNullOrEmpty(uncomittedPageFileDir, nameof(uncomittedPageFileDir));
			if (!Directory.Exists(uncomittedPageFileDir))
				throw new DirectoryNotFoundException($"Directory not found: {uncomittedPageFileDir}");
			FileID = fileID;
			PageMarkerRepo = new MarkerRepository(uncomittedPageFileDir, FileID);
		}

		public Guid FileID { get; }

		public override bool Dirty => base.Dirty || PageMarkerRepo.PageMarkers.Any() || PageMarkerRepo.FileMarkers.Any();

		public override void Load() {
			base.Load();

			// Resume prior commit if applicable
			if (PageMarkerRepo.Contains(FileMarkerType.Committing))
				Commit();

			// Resume prior rollback if applicable
			if (PageMarkerRepo.Contains(FileMarkerType.RollingBack))
				Rollback();
		}

		public void Commit() {
			var before = this.ToArray();
			Flush();
			var afterflush = this.ToArray();
			Debug.Assert(Enumerable.SequenceEqual(before, afterflush));
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
							var page = _pages[pageMarkers.Key];
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
			var afterLoad = this.ToArray();
			Debug.Assert(Enumerable.SequenceEqual(before, afterLoad));
		}

		public void Rollback() {
			Flush();
			PageMarkerRepo.Add(FileMarkerType.RollingBack);
			foreach (var page in _pages)
				page.HasUncommittedData = false;
			Clear();
			PageMarkerRepo.RemoveAllPageMarkers();
			PageMarkerRepo.RemoveAllFileMarkers();
			Load();
		}

		public override void Dispose() {
			if (PageMarkerRepo.FileMarkers.Any() || PageMarkerRepo.PageMarkers.Any())
				Rollback();
			base.Dispose();
		}

		protected abstract int GetComittedPageCount();

		protected override void OnPageCreating(int pageNumber) {
			base.OnPageCreating(pageNumber);
			if (Loading)
				return;

			// If creating a new page on a previously deleted page, 
			// remove deleted marker and add empty page marker.
			// This ensures the new page loads as empty (otherwise will fill from original)
			if (PageMarkerRepo.Contains(PageMarkerType.DeletedMarker, pageNumber)) {
				PageMarkerRepo.Add(PageMarkerType.UncommittedPage, pageNumber);
				PageMarkerRepo.Remove(PageMarkerType.DeletedMarker, pageNumber);
			}
		}

		protected override void OnPageSaving(TPage page) {
			base.OnPageSaving(page);

			// Clear any deleted marker
			if (PageMarkerRepo.Contains(PageMarkerType.DeletedMarker, page.Number))
				PageMarkerRepo.Remove(PageMarkerType.DeletedMarker, page.Number);

			if (!PageMarkerRepo.Contains(PageMarkerType.UncommittedPage, page.Number)) {
				// Add the page marker
				PageMarkerRepo.Add(PageMarkerType.UncommittedPage, page.Number);
			}
		}

		protected override void OnPageSaved(TPage page) {
			base.OnPageSaved(page);

			// Truncate last page
			if (page.Number == _pages.Count - 1) {
				Tools.FileSystem.TruncateFile(PageMarkerRepo.GetMarkerFilename(page.Number, PageMarkerType.UncommittedPage), page.Size);
			}

		}

		protected override void OnPageDeleting(TPage pageHeader) {
			base.OnPageDeleting(pageHeader);

			if (Disposing)
				return;
			// Create marker
			PageMarkerRepo.Remove(PageMarkerType.UncommittedPage, pageHeader.Number); // deletes uncommitted data
			var requiresDeletedMarker = pageHeader.Number < GetComittedPageCount();
			if (requiresDeletedMarker)
				PageMarkerRepo.Add(PageMarkerType.DeletedMarker, pageHeader.Number);
		}

		public abstract class TransactionalPageBase : FilePageBase {

			protected TransactionalPageBase(FileStream sourceFile, IObjectSizer<TItem> sizer, string uncommittedPageFileName, int pageNumber, int pageSize, IExtendedList<TItem> memoryStore)
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
				// create file marker
				if (!File.Exists(UncommittedPageFileName))
					throw new InvalidOperationException("Uncommitted page marker not created");

				// Write source data into the uncommitted page marker.
				// This marker is also the store for Uncommitted data.
				// When created, it contains the original source data.
				using (var readStream = OpenSourceReadStream())
					File.WriteAllBytes(UncommittedPageFileName, readStream.ReadBytes((int)readStream.Length));

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
				RemoveAllPageMakers(pageNumber, new[] { except });
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