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
/// TextWriter which writes to a file stream.
/// </summary>
/// <remarks></remarks>
public class FileTextWriter : TextWriterBase {

	/// <summary>
	/// This is the default encoding used by StreamWriter, which File.AppendAllText uses internally.
	/// </summary>
	private static Encoding _swEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

	private readonly TextWriter _textWriter;
	
	public FileTextWriter(string filePath, FileMode fileMode) 
		: this(new FileStream(filePath, fileMode, FileAccess.Write), _swEncoding) {
	}

	public FileTextWriter(FileStream fileStream, Encoding encoding) {
		_swEncoding = encoding;
		FileStream = fileStream;
		_textWriter = new StreamWriter(FileStream, encoding);
	}

	public FileStream FileStream { get; set; }

	protected override void Dispose(bool disposing) {
		_textWriter.Flush();
		FileStream.Flush();
		FileStream.Close();
		FileStream.Dispose();
		base.Dispose(disposing);
	}

	protected override void InternalWrite(string value) 
		 => _textWriter.Write(value);

	protected override Task InternalWriteAsync(string value) 
		 => _textWriter.WriteAsync(value);

}
