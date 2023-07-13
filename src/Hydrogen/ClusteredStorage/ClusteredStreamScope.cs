// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace Hydrogen;

public class ClusteredStreamScope : IDisposable {
	private readonly Action _closeAction;
	public ClusteredStreamScope(long recordIndex, ClusteredStreamRecord record, Action closeAction) {
		RecordIndex = recordIndex;
		Record = record;
		_closeAction = closeAction;
	}

	public long RecordIndex { get; }

	public ClusteredStreamRecord Record; // TODO: MAKE PROPERTY (check won't break when is struct)

	public Stream Stream;

	public void InvalidateCluster(long cluster) {
		((ClusteredStorage.FragmentProvider)((FragmentedStream)Stream).FragmentProvider).InvalidateCluster(cluster);
	}

	public void ResetTracking() {
		((ClusteredStorage.FragmentProvider)((FragmentedStream)Stream).FragmentProvider).Reset();
	}

	public void Dispose() {
		Stream?.Dispose();
		_closeAction?.Invoke();
	}
}
