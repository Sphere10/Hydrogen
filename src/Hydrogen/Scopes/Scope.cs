using System;

namespace Hydrogen {
	public class Scope : IScope {
        public event EventHandlerEx ScopeEnd;

        protected virtual void OnScopeEnd() {
        }

        private void NotifyScopeEnd() {
            OnScopeEnd();
            ScopeEnd?.Invoke();
        }

        public void Dispose() {
            NotifyScopeEnd();
        }
    }

	public class Scope<T> : Scope, IScope<T>  {
        public T Item { get; set; }
    }
}