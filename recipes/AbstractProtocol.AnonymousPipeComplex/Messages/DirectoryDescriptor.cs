// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace AbstractProtocol.AnonymousPipeComplex {
    [Serializable]
	public class DirectoryDescriptor : Descriptor {
        public DirectoryDescriptor(DirectoryInfo file) {
            base.Name = file.Name;
            base.CreatedOn = file.CreationTime;
        }

        internal static DirectoryDescriptor GenRandom() {
            var rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            var folders = Tools.FileSystem.GetSubDirectories(rootFolder);
            return new DirectoryDescriptor(new DirectoryInfo(folders.Length > 0 ? folders[0] : rootFolder));
        }
    }
}
