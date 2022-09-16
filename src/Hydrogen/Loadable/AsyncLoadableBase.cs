namespace Hydrogen;

/// <summary>
/// A <see cref="LoadableBase"/> abstraction that implements it's synchronous members using sync-over-async pattern.
/// </summary>
public abstract class AsyncLoadableBase : LoadableBase {
	protected override void LoadInternal() => LoadInternalAsync().WaitSafe();

	protected sealed override void OnLoading() => OnLoadingAsync().WaitSafe();

	protected sealed override void OnLoaded() => OnLoadedAsync().WaitSafe();
}
