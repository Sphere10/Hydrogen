using System;
using System.Threading;

namespace Hydrogen {

	/// <summary>
	/// A future whose value is fetched by a background thread and whose <see cref="Value"/> blocks until that thread complex.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BackgroundFetchedValue<T> : IFuture<T> {
		private readonly ManualResetEventSlim _resetEvent;
		private Exception _fetchError;
		private T _value;

		public BackgroundFetchedValue(Func<T> valueLoader) {
			_resetEvent = new ManualResetEventSlim(false);
			_value = default;
			_fetchError = null;
			ThreadPool.QueueUserWorkItem( _ => {
				try {
					_value = valueLoader();
				} catch (Exception error) {
					_fetchError = error;
				} finally {
					_resetEvent.Set();
				}
			});

		}

		public T Value {
			get {
				_resetEvent.Wait();
				if (_fetchError != null)
					throw new AggregateException(_fetchError);
				return _value;
			}
		}

		public static LazyLoad<T> From(Func<T> valueLoader) {
			return new LazyLoad<T>(valueLoader);
		}

		public override string ToString() {
			return _resetEvent.IsSet ? Convert.ToString(_value) : null;
		}
	}

}