namespace Sphere10.Framework {
    public sealed class ThreadSafe<T> : ThreadSafeObject {
		private T _value;

		public ThreadSafe(T @object) {
            _value = @object;
        }

        public T Value {
			get {
				using (EnterReadScope())
					return _value;
			}
			set {
				using (EnterWriteScope())
					_value = value;
			}
		}
	}
}