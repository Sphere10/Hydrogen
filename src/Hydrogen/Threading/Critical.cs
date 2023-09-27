namespace Hydrogen;

public sealed class Critical<T> : CriticalObject {
	private T _value;

	public Critical(T @object) {
		_value = @object;
	}

	public T Value {
		get {
			using (EnterAccessScope())
				return _value;
		}
		set {
			using (EnterAccessScope())
				_value = value;
		}
	}
}
