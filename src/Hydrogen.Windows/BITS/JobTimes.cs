// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Windows.BITS;

public class JobTimes {
	private BG_JOB_TIMES jobTimes;

	internal JobTimes(BG_JOB_TIMES jobTimes) {
		this.jobTimes = jobTimes;
	}

	public DateTime CreationTime {
		get { return Utils.FileTime2DateTime(jobTimes.CreationTime); }
	}

	public DateTime ModificationTime {
		get { return Utils.FileTime2DateTime(jobTimes.ModificationTime); }
	}

	public DateTime TransferCompletionTime {
		get { return Utils.FileTime2DateTime(jobTimes.TransferCompletionTime); }
	}
}
