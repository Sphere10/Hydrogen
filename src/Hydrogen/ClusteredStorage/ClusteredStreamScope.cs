using System;
using System.IO;

namespace Hydrogen;

public class ClusteredStreamScope : IDisposable {
	private readonly Action _closeAction;
	public ClusteredStreamScope(int recordIndex, ClusteredStreamRecord record, Action closeAction) {
		RecordIndex = recordIndex;
		Record = record;
		_closeAction = closeAction;
	}

	public int RecordIndex { get; }

	public ClusteredStreamRecord Record; // TODO: MAKE PROPERTY (check won't break when is struct)

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
