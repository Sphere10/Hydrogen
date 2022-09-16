using System.Threading.Tasks;

namespace Hydrogen;

public class SaveableDecorator<TSaveableImpl> : ISaveable where TSaveableImpl : ISaveable {
	public event EventHandlerEx<object> Saving { add => Internal.Saving += value; remove => Internal.Saving -= value; }
	public event EventHandlerEx<object> Saved { add => Internal.Saved += value; remove => Internal.Saved -= value; }

	protected readonly TSaveableImpl Internal;
	
	public SaveableDecorator(TSaveableImpl @internal) {
		Guard.ArgumentNotNull(@internal, nameof(@internal));
		Internal = @internal;
	}

	public virtual bool RequiresSave => Internal.RequiresSave;

	public virtual void Save() => Internal.Save();

	public virtual Task SaveAsync() => Internal.SaveAsync();
}

public class SaveableDecorator : SaveableDecorator<ISaveable> {
	public SaveableDecorator(ISaveable @internal) : base(@internal) {
	}
}