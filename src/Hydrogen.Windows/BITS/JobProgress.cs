// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.BITS;

public class JobProgress {
	private BG_JOB_PROGRESS jobProgress;

	internal JobProgress(BG_JOB_PROGRESS jobProgress) {
		this.jobProgress = jobProgress;
	}

	public ulong BytesTotal {
		get {
			if (this.jobProgress.BytesTotal == ulong.MaxValue)
				return 0;
			return this.jobProgress.BytesTotal;
		}
	}

	public ulong BytesTransferred {
		get { return this.jobProgress.BytesTransferred; }
	}

	public uint FilesTotal {
		get { return this.jobProgress.FilesTotal; }
	}

	public uint FilesTransferred {
		get { return this.jobProgress.FilesTransferred; }
	}
}
