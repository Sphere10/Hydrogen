// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.BITS;

public class BitsFileInfo {
	private BG_FILE_INFO fileInfo;

	internal BitsFileInfo(BG_FILE_INFO fileInfo) {
		this.fileInfo = fileInfo;
	}

	public BitsFileInfo(string remoteName, string localName) {
		this.fileInfo = new BG_FILE_INFO();
		this.fileInfo.RemoteName = remoteName;
		this.fileInfo.LocalName = localName;
	}

	public string RemoteName {
		get { return this.fileInfo.RemoteName; }
	}

	public string LocalName {
		get { return this.fileInfo.LocalName; }
	}


	internal BG_FILE_INFO _BG_FILE_INFO {
		get { return this.fileInfo; }
	}
}
