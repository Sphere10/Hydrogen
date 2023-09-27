using System;
using System.Linq.Expressions;

namespace Hydrogen.Mapping;
public class SingleMember : IAccessor {

	public SingleMember(Member member) {
		InnerMember = member;
	}

	#region Accessor Members

	public string Name => InnerMember.Name;

	public string FieldName => InnerMember.Name;

	public Type PropertyType => InnerMember.PropertyType;

	public Member InnerMember { get; }

	public IAccessor GetChildAccessor<T>(Expression<Func<T, object>> expression) {
		var property = expression.ToMember();
		return new PropertyChain(new[] { InnerMember, property });
	}

	public void SetValue(object target, object propertyValue) {
		InnerMember.SetValue(target, propertyValue);
	}

	public object GetValue(object target) {
		return InnerMember.GetValue(target);
	}

	#endregion

	public static SingleMember Build<T>(Expression<Func<T, object>> expression) {
		var member = expression.ToMember();
		return new SingleMember(member);
	}

	public static SingleMember Build<T>(string propertyName) {
		var member = typeof(T).GetProperty(propertyName).ToMember();
		return new SingleMember(member);
	}
}
