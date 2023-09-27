// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Diagnostics;

namespace Hydrogen.Windows.Security;

public class CopyableShare {
	private NTShare _sourceShare;
	private string _destPath;

	public CopyableShare(NTShare source) : this(source, source.ServerPath) {
	}

	public CopyableShare(NTShare source, string destPath) {
		SourceShare = source;
		DestPath = destPath;
	}


	public NTShare SourceShare {
		get { return _sourceShare; }
		set { _sourceShare = value; }
	}


	public string DestPath {
		get { return _destPath; }
		set { _destPath = value; }
	}

	public static CopyableShare[] FromNTShares(NTShare[] shares) {
		List<CopyableShare> copyableShares = new List<CopyableShare>();
		foreach (NTShare share in shares) {
			copyableShares.Add(new CopyableShare(share));
		}
		return copyableShares.ToArray();
	}

	public override string ToString() {
		Debug.Assert(SourceShare != null);
		return SourceShare.ToString();
	}


}
