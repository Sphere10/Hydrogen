using System;
using System.Linq.Expressions;

namespace Hydrogen.Mapping;

public interface IAccessor {
	string FieldName { get; }

	Type PropertyType { get; }
	Member InnerMember { get; }
	void SetValue(object target, object propertyValue);
	object GetValue(object target);

	IAccessor GetChildAccessor<T>(Expression<Func<T, object>> expression);

	string Name { get; }
}
