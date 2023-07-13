// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Linq;
using Hydrogen;

namespace AbstractProtocol.AnonymousPipeComplex;

[Serializable]
public class FolderContents {
	public Descriptor[] Items { get; init; }

	internal static FolderContents GenRandom() {
		var rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
		var folders = Tools.FileSystem.GetSubDirectories(rootFolder).Select(x => new DirectoryInfo(x)).ToArray();
		var files = Tools.FileSystem.GetFiles(rootFolder).Select(x => new FileInfo(x)).ToArray();
		return new FolderContents {
			Items =
				folders
					.Take(10)
					.Select(x => new DirectoryDescriptor(x))
					.Cast<Descriptor>()
					.Union(
						files
							.Take(10)
							.Select(x => new FileDescriptor(x))
					)
					.Randomize()
					.ToArray()
		};
	}
}
