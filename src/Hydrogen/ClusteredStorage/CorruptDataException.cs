// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class CorruptDataException : InvalidOperationException {

	public CorruptDataException(string msg)
		: base(msg) {
	}

	public CorruptDataException(ClusteredStorageHeader containerHeader, string msg)
		: this($"{msg} - [ClusteredStorage]: {containerHeader}") {
	}

	public CorruptDataException(ClusteredStorageHeader containerHeader, int cluster, string? msg = null)
		: this(containerHeader, $"Corrupt Cluster {cluster} ({msg ?? "Unexpected value in cluster envelope"})") {
	}
}
