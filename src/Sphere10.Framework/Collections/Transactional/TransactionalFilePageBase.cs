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
using System.Diagnostics;
using System.IO;

namespace Sphere10.Framework {

	public abstract class TransactionalFilePageBase<TItem> : FilePageBase<TItem>, ITransactionalFilePage<TItem> {

		protected TransactionalFilePageBase(FileStream sourceFile, IItemSizer<TItem> sizer, string uncommittedPageFileName, int pageNumber, int pageSize, IExtendedList<TItem> memoryStore)
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
}