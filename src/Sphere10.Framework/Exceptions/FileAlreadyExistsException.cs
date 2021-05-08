//-----------------------------------------------------------------------
// <copyright file="InternalErrorException.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework {
	public class FileAlreadyExistsException : SoftwareException {
        public FileAlreadyExistsException(string filename) 
            : this($"File already exists", filename) {

        }

		public FileAlreadyExistsException(string message, string filename)
			: base(message) {
            Path = filename;
		}

        public string Path { get; }
    }
}
