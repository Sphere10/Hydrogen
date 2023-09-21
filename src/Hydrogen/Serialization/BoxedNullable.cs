using System;
using System.Collections.Generic;

namespace Hydrogen;

internal class BoxedNullable<T> {

	private readonly T _value;

	public BoxedNullable(T value) {
		_value = value;
	}

	public T Value => _value;

	public bool HasValue => _value is not null;

	public override string ToString() {
		return _value?.ToString() ?? string.Empty;
	}

	public override bool Equals(object obj) {
		if (obj is BoxedNullable<T> other)
			return EqualityComparer<T>.Default.Equals(_value, other._value);
		return false;
	}

	public override int GetHashCode() {
		return HashCode.Combine(typeof(BoxedNullable<>).GetHashCode(), _value?.GetHashCode() ?? 0);
	}

	public static implicit operator BoxedNullable<T>(T value) {
		return new BoxedNullable<T>(value);
	}

	public static implicit operator T(BoxedNullable<T> value) {
		return value._value;
	}

	public static bool operator ==(BoxedNullable<T> a, BoxedNullable<T> b) {
		return a.Equals(b);
	}

	public static bool operator !=(BoxedNullable<T> a, BoxedNullable<T> b) {
		return !a.Equals(b);
	}
}

