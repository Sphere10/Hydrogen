// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;

namespace Hydrogen;

/// <summary>
/// Writes log messages to a file by appending through a <see cref="FileAppendTextWriter"/>.
/// </summary>
public class FileAppendLogger : TextWriterLogger {
	/// <summary>
	/// Creates a logger that appends to the specified file.
	/// </summary>
	/// <param name="file">Target file path.</param>
	public FileAppendLogger(string file) : this(file, false) {
	}

	/// <summary>
	/// Creates a logger that appends to the specified file, optionally creating the directory tree if missing.
	/// </summary>
	/// <param name="file">Target file path.</param>
	/// <param name="createDirectories">Create directories and the file if they do not exist.</param>
	public FileAppendLogger(string file, bool createDirectories)
		: base(new FileAppendTextWriter(file)) {
		if (createDirectories) {
			if (!File.Exists(file)) {
				Tools.FileSystem.CreateBlankFile(file, true);
			}
		}
	}
}
