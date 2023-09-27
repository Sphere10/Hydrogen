// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Windows.BITS;

public class FileProgress {
	private BG_FILE_PROGRESS fileProgress;

	internal FileProgress(BG_FILE_PROGRESS fileProgress) {
		this.fileProgress = fileProgress;
	}

	public ulong BytesTotal {
		get {
			if (this.fileProgress.BytesTotal == ulong.MaxValue)
				return 0;
			return this.fileProgress.BytesTotal;
		}
	}

	public ulong BytesTransferred {
		get { return this.fileProgress.BytesTransferred; }
	}

	public bool Completed {
		get { return Convert.ToBoolean(this.fileProgress.Completed); }
	}
}
