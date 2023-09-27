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
using System.Reflection;

namespace AbstractProtocol.AnonymousPipeComplex;

[Serializable]
public class FileDescriptor : Descriptor {
	public FileDescriptor(FileInfo file) {
		base.Name = file.Name;
		base.CreatedOn = file.CreationTime;
		Size = file.Length;
	}

	public long Size { get; init; }

	internal static FileDescriptor GenRandom() {
		var rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
		var files = Tools.FileSystem.GetFiles(rootFolder).ToArray();
		return new FileDescriptor(new FileInfo(files.Length > 0 ? files[0] : Assembly.GetExecutingAssembly().Location));
	}
}
