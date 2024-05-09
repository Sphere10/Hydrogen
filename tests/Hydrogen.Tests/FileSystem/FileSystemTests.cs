// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;
using System.IO;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
public class FileSystemTests {

	[Test]
	public void AvailableFile_1() {
		var path = Path.GetTempPath();
		var file = Guid.NewGuid().ToStrictAlphaString();
		var ext = ".ext";
		var expectedPath = Path.Combine(path, file + ext);
		ClassicAssert.AreEqual(expectedPath, Tools.FileSystem.DetermineAvailableFileName(path, file + ext));
	}

	[Test]
	public void AvailableFile_2() {
		var path = Path.GetTempPath();
		var file = Guid.NewGuid().ToStrictAlphaString();
		var ext = ".ext";
		var desiredPath = Path.Combine(path, file + ext);
		try {
			Tools.FileSystem.CreateBlankFile(desiredPath);
			ClassicAssert.AreEqual(Path.Combine(path, file + " 2" + ext), Tools.FileSystem.DetermineAvailableFileName(path, file + ext));
		} finally {
			File.Delete(Path.Combine(path, file + ext));
		}
	}

	[Test]
	public void AvailableFile_3() {
		var path = Path.GetTempPath();
		var file = Guid.NewGuid().ToStrictAlphaString();
		var ext = ".ext";
		try {
			Tools.FileSystem.CreateBlankFile(Path.Combine(path, file + ext));
			Tools.FileSystem.CreateBlankFile(Path.Combine(path, file + " 2" + ext));
			ClassicAssert.AreEqual(Path.Combine(path, file + " 3" + ext), Tools.FileSystem.DetermineAvailableFileName(path, file + ext));
		} finally {
			File.Delete(Path.Combine(path, file + ext));
			File.Delete(Path.Combine(path, file + " 2" + ext));
		}
	}


}
