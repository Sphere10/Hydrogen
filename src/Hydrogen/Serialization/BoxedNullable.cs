using System;
using System.Collections.Generic;

namespace Hydrogen;

internal class BoxedNullable<T> : IEquatable<BoxedNullable<T>> {

	private readonly object _value;

	public BoxedNullable() {
		_value = null;
	}

	public BoxedNullable(T value) {
		_value = value;
	}

	public T Value => (T)_value;

	public bool HasValue => _value is not null;

	public override string ToString() {
		return _value?.ToString() ?? string.Empty;
	}

	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj))
			return false;
		if (ReferenceEquals(this, obj))
			return true;
		if (obj.GetType() != this.GetType())
			return false;
		return Equals((BoxedNullable<T>)obj);
	}

	public bool Equals(BoxedNullable<T> other) {
		if (ReferenceEquals(null, other))
			return false;
		if (ReferenceEquals(this, other))
			return true;
		return Equals(_value, other._value);
	}

	public override int GetHashCode() {
		return (_value != null ? _value.GetHashCode() : 0);
	}

	public static implicit operator BoxedNullable<T>(T value) {
		return value is not null ? new BoxedNullable<T>(value) : new BoxedNullable<T>();
	}

	public static implicit operator T(BoxedNullable<T> value) {
		return value is not null ? (T)value._value : default;
	}

	public static bool operator ==(BoxedNullable<T> a, BoxedNullable<T> b) {
		return a.Equals(b);
	}

	public static bool operator !=(BoxedNullable<T> a, BoxedNullable<T> b) {
		return !a.Equals(b);
	}

}

