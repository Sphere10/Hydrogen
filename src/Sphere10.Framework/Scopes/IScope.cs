using System;

namespace Sphere10.Framework {
	public interface IScope : IDisposable {
        event EventHandler ScopeEnd;
    }

	public interface IScope<out T> : IScope {
		public T Item { get; }
	}
}