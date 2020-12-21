using System;

namespace Sphere10.Framework {

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
				if (!_loaded) {
					_value = _loader();
					_loaded = true;
				}
				return _value;
			}
		}

		public static LazyLoad<T> From(Func<T> valueLoader) {
			return new LazyLoad<T>(valueLoader);
		}
	}

}