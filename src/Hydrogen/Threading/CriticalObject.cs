using System;
using System.Threading;

namespace Hydrogen;

public class CriticalObject : ICriticalObject {
	
	private readonly object _lock;

	public CriticalObject() : this(new object()) {
	}

	public CriticalObject(object @lock) {
		Guard.ArgumentNotNull(@lock, nameof(@lock));
		_lock = @lock; 
	}

	public ICriticalObject ParentCriticalObject { get; set; }

	public object Lock => ParentCriticalObject?.Lock ?? _lock;

	public bool IsLocked => Monitor.IsEntered(Lock);

	public IDisposable EnterAccessScope() {
		var lockObj = Lock;
		Monitor.Enter(lockObj);
		OnAccessScopeOpen();
		var scope = new ActionDisposable(
			() => {
				Monitor.Exit(lockObj);
				OnAccessScopeClosed();
			}
		);
		return scope;
	}

	protected virtual void OnAccessScopeOpen() {
	}

	protected virtual void OnAccessScopeClosed() {
	}

}