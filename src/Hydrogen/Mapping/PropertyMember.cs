using System;
using System.Reflection;

namespace Hydrogen.Mapping;

[Serializable]
internal class PropertyMember : Member {
	private readonly PropertyInfo _member;
	private readonly MethodMember _getMethod;
	private readonly MethodMember _setMethod;
	private Member _backingField;

	public PropertyMember(PropertyInfo member) {
		_member = member;
		_getMethod = GetMember(member.GetGetMethod(true));
		_setMethod = GetMember(member.GetSetMethod(true));
	}

	MethodMember GetMember(MethodInfo method) {
		if (method == null)
			return null;

		return (MethodMember)method.ToMember();
	}

	public override void SetValue(object target, object value) {
		_member.SetValue(target, value, null);
	}

	public override object GetValue(object target) {
		return _member.GetValue(target, null);
	}

	public override bool TryGetBackingField(out Member field) {
		if (_backingField != null) {
			field = _backingField;
			return true;
		}

		var reflectedField = DeclaringType.GetField(Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		reflectedField = reflectedField ?? DeclaringType.GetField("_" + Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		reflectedField = reflectedField ?? DeclaringType.GetField("m_" + Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

		if (reflectedField == null) {
			field = null;
			return false;
		}

		field = _backingField = new FieldMember(reflectedField);
		return true;
	}

	public override string Name => _member.Name;

	public override Type PropertyType => _member.PropertyType;

	public override bool CanRead => _getMethod is not null && _getMethod.IsPublic && _member.CanRead;

	public override bool CanWrite => _setMethod is not null && _setMethod.IsPublic && _member.CanWrite;

	public override MemberInfo MemberInfo => _member;

	public override Type DeclaringType => _member.DeclaringType;

	public override bool HasIndexParameters => _member.GetIndexParameters().Length > 0;

	public override bool IsMethod => false;

	public override bool IsField => false;

	public override bool IsProperty => true;

	public override bool IsAutoProperty 
		=> _getMethod != null && _getMethod.IsCompilerGenerated || _setMethod != null && _setMethod.IsCompilerGenerated;

	public override bool IsPrivate => _getMethod.IsPrivate;

	public override bool IsProtected => _getMethod.IsProtected;

	public override bool IsPublic => _getMethod.IsPublic;

	public override bool IsInternal => _getMethod.IsInternal;

	public MethodMember Get => _getMethod;

	public MethodMember Set => _setMethod;

	public override string ToString() {
		return "{Property: " + _member.Name + "}";
	}
}
