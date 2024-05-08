using System;
using System.Reflection;

namespace Hydrogen.Mapping;

[Serializable]
public abstract class Member : IEquatable<Member> {
	public abstract string Name { get; }
	public abstract Type PropertyType { get; }
	public abstract bool CanRead { get; }
	public abstract bool CanWrite { get; }
	public abstract MemberInfo MemberInfo { get; }
	public abstract Type DeclaringType { get; }
	public abstract bool HasIndexParameters { get; }
	public abstract bool IsMethod { get; }
	public abstract bool IsField { get; }
	public abstract bool IsProperty { get; }
	public abstract bool IsAutoProperty { get; }
	public abstract bool IsPrivate { get; }
	public abstract bool IsProtected { get; }
	public abstract bool IsPublic { get; }
	public abstract bool IsInternal { get; }

	public bool Equals(Member other) {
		return Equals(other.MemberInfo.MetadataToken, MemberInfo.MetadataToken) && Equals(other.MemberInfo.Module, MemberInfo.Module);
	}

	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (!(obj is Member)) return false;
		return Equals((Member)obj);
	}

	public override int GetHashCode() {
		return MemberInfo.GetHashCode() ^ 3;
	}

	public static bool operator ==(Member left, Member right) {
		return Equals(left, right);
	}

	public static bool operator !=(Member left, Member right) {
		return !Equals(left, right);
	}

	public abstract void SetValue(object target, object value);

	public abstract object GetValue(object target);

	public abstract bool TryGetBackingField(out Member backingField);

	public Func<TItem, TProperty> AsFunc<TItem, TProperty>() {
		Guard.Ensure(DeclaringType == typeof(TItem) && PropertyType == typeof(TProperty), "Generic types TItem and TProperty must match the member's declaring type and property type.");
		return item => {
			Guard.ArgumentNotNull(item, nameof(item));
			var value = GetValue(item);
			return (TProperty)value;
		};
	}

	public Delegate AsDelegate() {
		var method = GetType()
			.GetMethod(nameof(AsFunc), BindingFlags.Public | BindingFlags.Instance)
			.MakeGenericMethod(DeclaringType, PropertyType);
		return (Delegate)method.Invoke(this, null);
	}

}
