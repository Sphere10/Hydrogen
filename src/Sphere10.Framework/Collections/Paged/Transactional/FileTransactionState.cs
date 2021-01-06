namespace Sphere10.Framework {

	public enum FileTransactionState {
		Unchanged,
		HasChanges,
		Committing,
		RollingBack,
	}

}