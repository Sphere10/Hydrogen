// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Linq;

namespace Hydrogen.Data;

public static class IFileStoreExtensions {
	public static string RegisterFile<TFileKeyType>(this IFileStore<TFileKeyType> fileStoreBase, TFileKeyType fileKey) {
		return fileStoreBase.RegisterMany(new[] { fileKey }).Single();
	}

	public static void Delete<TFileKeyType>(this IFileStore<TFileKeyType> fileStoreBase, TFileKeyType fileKey) {
		fileStoreBase.DeleteMany(new[] { fileKey });
	}
}
