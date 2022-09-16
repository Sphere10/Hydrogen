namespace Hydrogen;

/// <summary>
/// A <see cref="SaveableBase"/> abstraction that implements it's synchronous members using sync-over-async pattern.
/// </summary>
public abstract class AsyncSaveableBase : SaveableBase {
	
	protected override void SaveInternal() => SaveInternalAsync().WaitSafe();

	protected sealed override void OnSaving() => OnSavingAsync().WaitSafe();

	protected sealed override void OnSaved() => OnSavedAsync().WaitSafe();
}
