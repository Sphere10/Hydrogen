using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// Base implementation of <see cref="IPersistable"/>.
/// </summary>
public abstract class PersistableBase : LoadableBase, IPersistable {
	// NOTE: Since multiple inheritance is disallowed in C#, we inherit from LoadableBase and
	// copy-paste of SaveableBase below removing common members.

	public event EventHandlerEx<object> Saving;
	public event EventHandlerEx<object> Saved;
	
	public virtual bool RequiresSave { get; set; } = false;

	public void Save() {
		NotifySaving();
		SaveInternal();
		NotifySaved();
	}

	public async Task SaveAsync() {
		await NotifySavingAsync();
		await SaveInternalAsync();
		await NotifySavedAsync();
	}

	protected abstract void SaveInternal();

	protected abstract Task SaveInternalAsync();

	protected virtual void OnSaving() {
	}

	protected virtual void OnSaved() {
	}

	protected virtual Task OnSavingAsync() => Task.CompletedTask;

	protected virtual Task OnSavedAsync() => Task.CompletedTask;

	protected void NotifySaving() {
		if (SuppressNotifications)
			return;
		OnSaving();
		Saving?.Invoke(this);
	}

	protected async Task NotifySavingAsync() {
		if (SuppressNotifications)
			return;
		await OnSavingAsync();
		await Task.Run( () => Saving?.Invoke(this));
	}

	protected void NotifySaved() {
		if (SuppressNotifications)
			return;
		OnSaved();
		Saved?.Invoke(this);

	}

	protected async Task NotifySavedAsync() {
		if (SuppressNotifications)
			return;
		await OnSavedAsync();
		await Task.Run( () => Saved?.Invoke(this));
	}
}
