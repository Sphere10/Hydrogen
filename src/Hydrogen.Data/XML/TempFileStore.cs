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

	public class XmlTempFileStore : XmlFileStore {

		public XmlTempFileStore() : this(Guid.NewGuid().ToStrictAlphaString()) {
			
		}

		public XmlTempFileStore(string subDir) : base(Path.Combine(Path.GetTempPath(), subDir), new NotPersistedDictionary<string,string>(), XmlFileStorePersistencePolicy.DeleteOnDispose) {
		}

		protected override string GenerateInternalFilePath(string fileAlias) {
			return Path.Combine(base.BaseDirectory, Path.GetFileName(Guid.NewGuid().ToStrictAlphaString()));
		}

	}
}
