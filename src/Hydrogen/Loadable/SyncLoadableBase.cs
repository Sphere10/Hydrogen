using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A <see cref="LoadableBase"/> abstraction that implements it's asynchronous members using <see cref="Task.Run(System.Action)"/>.
/// </summary>
public abstract class SyncLoadableBase : LoadableBase {
	protected override Task LoadInternalAsync() => Task.Run(LoadInternal);

	protected sealed override Task  OnLoadingAsync() => Task.Run(OnLoading);

	protected sealed override Task OnLoadedAsync() => Task.Run(OnLoaded);
}
