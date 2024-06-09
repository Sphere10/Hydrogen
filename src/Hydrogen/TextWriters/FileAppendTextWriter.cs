// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// TextWriter which appends to a file.
/// </summary>
/// <remarks></remarks>
public class FileAppendTextWriter : TextWriterBase {

	/// <summary>
	/// This is the default encoding used by StreamWriter, which File.AppendAllText uses internally.
	/// </summary>
	private static Encoding _swEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

	/// <summary>
	/// Initializes a new instance of the <see cref="FileAppendTextWriter"/> class.
	/// </summary>
	/// <param name="filePath">The file path.</param>
	/// <remarks></remarks>
	public FileAppendTextWriter(string filePath, bool createIfMissing = false) : this(filePath, _swEncoding, createIfMissing) {
		FilePath = filePath;
		if (createIfMissing && !File.Exists(FilePath))
			Tools.FileSystem.CreateBlankFile(FilePath, true);
	}

	public FileAppendTextWriter(string filePath, Encoding encoding, bool createIfMissing) {
		_swEncoding = encoding;
		FilePath = filePath;
		if (createIfMissing && !File.Exists(FilePath))
			Tools.FileSystem.CreateBlankFile(FilePath, true);
	}

	public string FilePath { get; set; }

	protected override void InternalWrite(string value)
		=> File.AppendAllText(FilePath, value, _swEncoding);

	protected override Task InternalWriteAsync(string value)
		=> File.AppendAllTextAsync(FilePath, value, _swEncoding);

}
