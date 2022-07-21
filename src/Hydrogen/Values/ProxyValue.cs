using System;

namespace Hydrogen;

/// <summary>
/// A future whose value is fetched on every request.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ProxyValue<T> : IFuture<T> {
	private readonly Func<T> _loader;

	public ProxyValue(Func<T> valueLoader) {
		_loader = valueLoader;
	}

	public T Value => _loader();

	public static ProxyValue<T> From(Func<T> valueLoader) {
		return new ProxyValue<T>(valueLoader);
	}
	public override string ToString() => Value.ToString();

}
