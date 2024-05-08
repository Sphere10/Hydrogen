using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Hydrogen.Mapping;

[Serializable]
internal class MethodMember : Member {
	private readonly MethodInfo _member;
	private Member _backingField;

	public override void SetValue(object target, object value) {
		throw new NotSupportedException("Cannot set the value of a method Member.");
	}

	public override object GetValue(object target) {
		return _member.Invoke(target, null);
	}

	public override bool TryGetBackingField(out Member field) {
		if (_backingField != null) {
			field = _backingField;
			return true;
		}

		var name = Name;

		if (name.StartsWith("Get", StringComparison.InvariantCultureIgnoreCase))
			name = name.Substring(3);

		var reflectedField = DeclaringType.GetField(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		reflectedField ??= DeclaringType.GetField("_" + name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		reflectedField ??= DeclaringType.GetField("m_" + name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

		if (reflectedField == null) {
			field = null;
			return false;
		}

		field = _backingField = new FieldMember(reflectedField);
		return true;
	}

	public MethodMember(MethodInfo member) {
		this._member = member;
	}

	public override string Name => _member.Name;

	public override Type PropertyType => _member.ReturnType;

	public override bool CanRead => false;

	public override bool CanWrite => false;

	public override MemberInfo MemberInfo => _member;

	public override Type DeclaringType => _member.DeclaringType;

	public override bool HasIndexParameters => false;

	public override bool IsMethod => true;

	public override bool IsField => false;

	public override bool IsProperty => false;

	public override bool IsAutoProperty => false;

	public override bool IsPrivate => _member.IsPrivate;

	public override bool IsProtected => _member.IsFamily || _member.IsFamilyAndAssembly;

	public override bool IsPublic => _member.IsPublic;

	public override bool IsInternal => _member.IsAssembly || _member.IsFamilyAndAssembly;

	public bool IsCompilerGenerated => _member.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any();

	public override string ToString() {
		return "{Method: " + _member.Name + "}";
	}
}
