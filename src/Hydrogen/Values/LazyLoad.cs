using System;

namespace Hydrogen {

	/// <summary>
	/// A future whose value is fetched on first rqeuest and retained for further requests.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class LazyLoad<T> : IFuture<T> {
		private bool _loaded;
		private T _value;
		private readonly Func<T> _loader;

		public LazyLoad(Func<T> valueLoader) {
			_loader = valueLoader;
			_loaded = false;
			_value = default;
		}

		public T Value {
			get {
				if (_loaded)
					return _value;
				_value = _loader();
				_loaded = true;
				return _value;
			}
		}

		public static LazyLoad<T> From(Func<T> valueLoader) {
			return new LazyLoad<T>(valueLoader);
		}

		public override string ToString() {
			return _loaded ? Convert.ToString(_value) : null;
		}
	}

}