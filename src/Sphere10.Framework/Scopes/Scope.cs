using System;

namespace Sphere10.Framework {
    public class Scope : IScope {
        public event EventHandler ScopeEnd;

        protected virtual void OnScopeEnd() {
        }

        private void NotifyScopeEnd() {
            OnScopeEnd();
            ScopeEnd?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose() {
            NotifyScopeEnd();
        }
    }

	public class Scope<T> : Scope, IScope<T>  {
        public T Item { get; set; }
    }
}