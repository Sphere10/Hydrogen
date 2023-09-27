// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.Windows.BITS;

public class BitsFiles : List<BitsFile>, IDisposable {
	private IEnumBackgroundCopyFiles fileList;
	private BitsJob job;
	private bool disposed;

	internal BitsFiles(BitsJob job, IEnumBackgroundCopyFiles fileList) {
		this.fileList = fileList;
		this.job = job;
		this.Refresh();
	}

	internal void Refresh() {
		uint count;
		IBackgroundCopyFile currentFile;
		uint fetchedCount = 0;
		this.fileList.Reset();
		this.Clear();
		this.fileList.GetCount(out count);
		for (int i = 0; i < count; i++) {
			this.fileList.Next(1, out currentFile, out fetchedCount);
			if (fetchedCount == 1) {
				this.Add(new BitsFile(this.job, currentFile));
			}
		}
	}

	#region IDisposable Members

	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing) {
		if (!this.disposed) {
			if (disposing) {
				//TODO: release COM resource
				this.fileList = null;
			}
		}
		disposed = true;
	}

	#endregion

}
