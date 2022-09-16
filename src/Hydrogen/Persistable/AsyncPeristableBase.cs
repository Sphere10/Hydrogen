namespace Hydrogen;

/// <summary>
/// A <see cref="PersistableBase"/> abstraction that implements it's synchronous members using sync-over-async pattern.
/// </summary>
public abstract class AsyncPeristableBase : PersistableBase {

	// Copy of AsyncLoadableBase
	protected override void LoadInternal() => LoadInternalAsync().WaitSafe();

	protected sealed override void OnLoading() => OnLoadingAsync().WaitSafe();

	protected sealed override void OnLoaded() => OnLoadedAsync().WaitSafe();


	// Copy of AsyncSaveableBase
	protected override void SaveInternal() => SaveInternalAsync().WaitSafe();

	protected sealed override void OnSaving() => OnSavingAsync().WaitSafe();

	protected sealed override void OnSaved() => OnSavedAsync().WaitSafe();
}
