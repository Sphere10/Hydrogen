using System;

namespace Hydrogen;

public interface IScope : IDisposable, IAsyncDisposable {
	event EventHandlerEx ScopeEnd;
}

public interface IScope<out T> : IScope {
	public T Item { get; }
}