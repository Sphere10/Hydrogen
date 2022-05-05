//-----------------------------------------------------------------------
// <copyright file="JobTimes.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Windows.BITS
{
    public class JobTimes
    {
        private BG_JOB_TIMES jobTimes;

        internal JobTimes(BG_JOB_TIMES jobTimes)
        {
            this.jobTimes = jobTimes;
        }

        public DateTime CreationTime
        {
            get { return Utils.FileTime2DateTime(jobTimes.CreationTime); }
        }

        public DateTime ModificationTime
        {
            get { return Utils.FileTime2DateTime(jobTimes.ModificationTime); }
        }

        public DateTime TransferCompletionTime
        {
            get { return Utils.FileTime2DateTime(jobTimes.TransferCompletionTime); }
        }
    }
}
