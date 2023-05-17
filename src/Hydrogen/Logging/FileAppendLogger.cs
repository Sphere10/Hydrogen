// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;

namespace Hydrogen;
public class FileAppendLogger : TextWriterLogger {
	public FileAppendLogger(string file) : this(file, false) {
	}

	public FileAppendLogger(string file, bool createDirectories)
		: base(new FileAppendTextWriter(file)) {
		if (createDirectories) {
			if (!File.Exists(file)) {
				Tools.FileSystem.CreateBlankFile(file, true);
			}
		}
	}
}

