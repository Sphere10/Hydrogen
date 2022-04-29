using System;

namespace Hydrogen {
	public interface IScope : IDisposable {
        event EventHandlerEx ScopeEnd;
    }

	public interface IScope<out T> : IScope {
		public T Item { get; }
	}
}