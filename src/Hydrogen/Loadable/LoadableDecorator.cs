using System.Threading.Tasks;

namespace Hydrogen;

public class LoadableDecorator<TLoadableImpl> : ILoadable where TLoadableImpl : ILoadable {
	public event EventHandlerEx<object> Loading { add => Internal.Loading += value; remove => Internal.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => Internal.Loaded += value; remove => Internal.Loaded -= value; }

	protected readonly TLoadableImpl Internal;
	
	public LoadableDecorator(TLoadableImpl @internal) {
		Guard.ArgumentNotNull(@internal, nameof(@internal));
		Internal = @internal;
	}

	public virtual bool RequiresLoad => Internal.RequiresLoad;

	public virtual void Load() => Internal.Load();

	public virtual Task LoadAsync() => Internal.LoadAsync();
}


public class LoadableDecorator : LoadableDecorator<ILoadable>  {
	public LoadableDecorator(ILoadable @internal) : base(@internal) {
	}
}