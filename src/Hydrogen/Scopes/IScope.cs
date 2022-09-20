using System;

namespace Hydrogen;

public interface IScope : IAsyncDisposable, IDisposable {
	event EventHandlerEx ScopeEnd;
}

public interface IScope<out T> : IScope {
	public T Item { get; }
}