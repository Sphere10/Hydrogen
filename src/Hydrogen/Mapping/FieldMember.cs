using System;
using System.Reflection;

namespace Hydrogen.Mapping;

[Serializable]
internal class FieldMember : Member {
	private readonly FieldInfo _member;

	public override void SetValue(object target, object value) {
		_member.SetValue(target, value);
	}

	public override object GetValue(object target) {
		return _member.GetValue(target);
	}

	public override bool TryGetBackingField(out Member backingField) {
		backingField = null;
		return false;
	}

	public FieldMember(FieldInfo member) {
		_member = member;
	}

	public override string Name => _member.Name;

	public override Type PropertyType => _member.FieldType;

	public override bool CanRead => IsPublic;

	public override bool CanWrite => IsPublic;

	public override MemberInfo MemberInfo => _member;

	public override Type DeclaringType => _member.DeclaringType;

	public override bool HasIndexParameters => false;

	public override bool IsMethod => false;

	public override bool IsField => true;

	public override bool IsProperty => false;

	public override bool IsAutoProperty => false;

	public override bool IsPrivate => _member.IsPrivate;

	public override bool IsProtected => _member.IsFamily || _member.IsFamilyAndAssembly;

	public override bool IsPublic => _member.IsPublic;

	public override bool IsInternal => _member.IsAssembly || _member.IsFamilyAndAssembly;

	public override string ToString() {
		return "{Field: " + _member.Name + "}";
	}
}
