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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sphere10.Framework {

	/// <summary>
	/// TextWriter which appends to a file.
	/// </summary>
	/// <remarks></remarks>
	public class FileAppendTextWriter : BaseTextWriter {


		/// <summary>
		/// Initializes a new instance of the <see cref="FileAppendTextWriter"/> class.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <remarks></remarks>
		public FileAppendTextWriter(string filePath, bool createIfMissing = false) {
			FilePath = filePath;
            if (createIfMissing && !File.Exists(FilePath))
                Tools.FileSystem.CreateBlankFile(FilePath, true);
		}

		public string FilePath { get; set; }
        
		protected override void InternalWrite(string value) {
			File.AppendAllText(FilePath, value);
		}

	}

}
