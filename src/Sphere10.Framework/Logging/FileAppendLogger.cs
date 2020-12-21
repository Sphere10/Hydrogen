//-----------------------------------------------------------------------
// <copyright file="FileAppendLogger.cs" company="Sphere 10 Software">
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
using System.IO;
using System.Linq;
using System.Text;

namespace Sphere10.Framework {
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
}
