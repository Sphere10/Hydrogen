using System;
using System.IO;

namespace Sphere10.Framework;

public class ClusteredStorageScope : IDisposable {
	private readonly Action _closeAction;
	public ClusteredStorageScope(int recordIndex, ClusteredStorageRecord record, Action closeAction) {
		RecordIndex = recordIndex;
		Record = record;
		_closeAction = closeAction;
	}

	public int RecordIndex;

	public ClusteredStorageRecord Record;

	public Stream Stream;

	public void InvalidateCluster(int cluster) {
		((ClusteredStorage.FragmentProvider)((FragmentedStream)Stream).FragmentProvider).InvalidateCluster(cluster);
	}

	public void  ResetTracking() {
		((ClusteredStorage.FragmentProvider)((FragmentedStream)Stream).FragmentProvider).Reset();
	}

	public void Dispose() {
		Stream?.Dispose();
		_closeAction?.Invoke();
	}
}
