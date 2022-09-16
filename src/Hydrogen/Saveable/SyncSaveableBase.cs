using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A <see cref="SaveableBase"/> abstraction that implements it's asynchronous members using <see cref="Task.Run(System.Action)"/>.
/// </summary>
public abstract class SyncSaveableBase : SaveableBase {
	
	protected override Task SaveInternalAsync() => Task.Run(SaveInternal);

	protected sealed override Task OnSavingAsync() => Task.Run(OnSaving);

	protected sealed override Task OnSavedAsync() => Task.Run(OnSaved);
}
