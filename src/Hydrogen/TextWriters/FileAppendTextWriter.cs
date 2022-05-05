//-----------------------------------------------------------------------
// <copyright file="FileAppendTextWriter.cs" company="Sphere 10 Software">
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

using System.IO;
using System.Text;

namespace Hydrogen {

	/// <summary>
	/// TextWriter which appends to a file.
	/// </summary>
	/// <remarks></remarks>
	public class FileAppendTextWriter : BaseTextWriter {

		/// <summary>
		/// This is the default encoding used by StreamWriter, which File.AppendAllText uses internally.
		/// </summary>
		private static Encoding _swEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
		
		/// <summary>
		/// Initializes a new instance of the <see cref="FileAppendTextWriter"/> class.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <remarks></remarks>
		public FileAppendTextWriter(string filePath, bool createIfMissing = false) : this(filePath,  _swEncoding, createIfMissing) {
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
        
		protected override void InternalWrite(string value) {
			File.AppendAllText(FilePath, value, _swEncoding);
		}
	}
}
