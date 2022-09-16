using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// Decorator for an <see cref="IPersistable"/> implementation.
/// </summary>
/// <typeparam name="TPersistableImpl"></typeparam>
public class PersistableDecorator<TPersistableImpl> : LoadableDecorator<TPersistableImpl> where TPersistableImpl : IPersistable {
	// NOTE: Since multiple inheritance is disallowed in C#, we inherit from LoadableBase and
	// copy-paste of SaveableDecorator below removing common members.

	public event EventHandlerEx<object> Saving { add => Internal.Saving += value; remove => Internal.Saving -= value; }
	public event EventHandlerEx<object> Saved { add => Internal.Saved += value; remove => Internal.Saved -= value; }

	public PersistableDecorator(TPersistableImpl internalPersistable) : base(internalPersistable) {
	}

	public virtual bool RequiresSave => Internal.RequiresSave;

	public virtual void Save() => Internal.Save();

	public virtual Task SaveAsync() => Internal.SaveAsync();


}
