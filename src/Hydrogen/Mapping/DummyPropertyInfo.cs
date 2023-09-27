using System;
using System.Globalization;
using System.Reflection;

namespace Hydrogen.Mapping;

[Serializable]
public sealed class DummyPropertyInfo : PropertyInfo {
	private readonly string _name;
	private readonly Type _type;

	public DummyPropertyInfo(string name, Type type) {
		Guard.ArgumentNotNullOrEmpty(name, nameof(name));
		Guard.ArgumentNotNull(type, nameof(type));
		_name = name;
		_type = type;
	}

	public override Module Module => null;

	public override int MetadataToken => _name.GetHashCode();

	public override object[] GetCustomAttributes(bool inherit) => Array.Empty<object>();

	public override bool IsDefined(Type attributeType, bool inherit) => false;

	public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) => obj;

	public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) {
	}

	public override MethodInfo[] GetAccessors(bool nonPublic) => Array.Empty<MethodInfo>();

	public override MethodInfo GetGetMethod(bool nonPublic) => null;

	public override MethodInfo GetSetMethod(bool nonPublic) => null;

	public override ParameterInfo[] GetIndexParameters() => Array.Empty<ParameterInfo>();

	public override string Name => _name;

	public override Type DeclaringType => _type;

	public override Type ReflectedType => null;

	public override Type PropertyType => _type;

	public override PropertyAttributes Attributes => PropertyAttributes.None;

	public override bool CanRead => false;

	public override bool CanWrite => false;

	public override object[] GetCustomAttributes(Type attributeType, bool inherit) => Array.Empty<object>();
}
