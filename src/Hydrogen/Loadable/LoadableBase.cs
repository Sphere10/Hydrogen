using System.Threading.Tasks;

namespace Hydrogen;

public abstract class LoadableBase : ILoadable {
	public event EventHandlerEx<object> Loading;
	public event EventHandlerEx<object> Loaded;
	
	public virtual bool RequiresLoad { get; set; } = false;

	protected virtual bool SuppressNotifications { get; set; } = false;

	public void Load() {
		NotifyLoading();
		LoadInternal();
		NotifyLoaded();
	}

	public async Task LoadAsync() {
		await NotifyLoadingAsync();
		await LoadInternalAsync();
		await NotifyLoadedAsync();
	}

	protected abstract void LoadInternal();

	protected abstract Task LoadInternalAsync();

	protected virtual void OnLoading() {
	}

	protected virtual void OnLoaded() {
	}

	protected virtual Task OnLoadingAsync() => Task.CompletedTask;

	protected virtual Task OnLoadedAsync() => Task.CompletedTask;

	protected void NotifyLoading() {
		if (SuppressNotifications)
			return;
		OnLoading();
		Loading?.Invoke(this);
	}

	protected async Task NotifyLoadingAsync() {
		if (SuppressNotifications)
			return;
		await OnLoadingAsync();
		await Task.Run( () => Loading?.Invoke(this));
	}

	protected void NotifyLoaded() {
		if (SuppressNotifications)
			return;
		OnLoaded();
		Loaded?.Invoke(this);

	}

	protected async Task NotifyLoadedAsync() {
		if (SuppressNotifications)
			return;
		await OnLoadedAsync();
		await Task.Run( () => Loaded?.Invoke(this));
	}
}
