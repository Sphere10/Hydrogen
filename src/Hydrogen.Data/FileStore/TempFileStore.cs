//-----------------------------------------------------------------------
// <copyright file="TempFileStore.cs" company="Sphere 10 Software">
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
using System.IO;

namespace Hydrogen.Data {

	/// <summary>
	/// A <see cref="SimpleFileStore"/> that only keeps file ephemerally (will delete on dispose).
	/// </summary>
	public class TempFileStore : SimpleFileStore {

		public TempFileStore() : this(Guid.NewGuid().ToStrictAlphaString()) {
		}

		public TempFileStore(string subDir) : base(Path.Combine(Path.GetTempPath(), subDir), new NotPersistedDictionary<string,string>(), FileStorePersistencePolicy.DeleteOnDispose) {
		}

		protected override string GenerateInternalRelFilePath(string fileKey) {
			return Path.Combine(base.BaseDirectory, Path.GetFileName(Guid.NewGuid().ToStrictAlphaString()));
		}

	}
}
