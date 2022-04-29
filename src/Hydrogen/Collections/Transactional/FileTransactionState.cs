namespace Hydrogen {

	public enum FileTransactionState {
		Unchanged,
		HasChanges,
		Committing,
		RollingBack,
	}

}