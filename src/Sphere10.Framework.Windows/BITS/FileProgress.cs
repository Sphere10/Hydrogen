//-----------------------------------------------------------------------
// <copyright file="FileProgress.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Windows.BITS
{
    public class FileProgress
    {
        private BG_FILE_PROGRESS fileProgress;

        internal FileProgress(BG_FILE_PROGRESS fileProgress)
        {
            this.fileProgress = fileProgress;
        }

        public ulong BytesTotal
        {
            get
            {
                if (this.fileProgress.BytesTotal == ulong.MaxValue)
                    return 0;
                return this.fileProgress.BytesTotal; 
            }
        }

        public ulong BytesTransferred
        {
            get { return this.fileProgress.BytesTransferred; }
        }

        public bool Completed
        {
            get { return Convert.ToBoolean(this.fileProgress.Completed); }
        }
    }
}
