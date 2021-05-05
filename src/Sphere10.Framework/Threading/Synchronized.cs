namespace Sphere10.Framework {
    public sealed class Synchronized<T> : SynchronizedObject {
		private T _value;

		public Synchronized(T @object) {
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