//-----------------------------------------------------------------------
// <copyright file="JobProgress.cs" company="Sphere 10 Software">
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
using System.Text;

namespace Sphere10.Framework.Windows.BITS
{
    public class JobProgress
    {
        private BG_JOB_PROGRESS jobProgress;

        internal JobProgress(BG_JOB_PROGRESS jobProgress)
        {
            this.jobProgress = jobProgress;
        }

        public ulong BytesTotal
        {
            get 
            {
                if (this.jobProgress.BytesTotal == ulong.MaxValue)
                    return 0;
                return this.jobProgress.BytesTotal; 
            }
        }

        public ulong BytesTransferred
        {
            get { return this.jobProgress.BytesTransferred;  }
        }

        public uint FilesTotal
        {
            get { return this.jobProgress.FilesTotal;  }
        }

        public uint FilesTransferred
        {
            get { return this.jobProgress.FilesTransferred; }
        }
    }
}
