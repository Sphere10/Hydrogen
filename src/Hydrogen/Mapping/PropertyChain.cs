using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hydrogen.Mapping;

public class PropertyChain : IAccessor {
	private readonly Member[] _chain;
	private readonly SingleMember _innerMember;

	public PropertyChain(Member[] members) {
		_chain = new Member[members.Length - 1];
		for (int i = 0; i < _chain.Length; i++) {
			_chain[i] = members[i];
		}

		_innerMember = new SingleMember(members[members.Length - 1]);
	}

	#region Accessor Members

	public void SetValue(object target, object propertyValue) {
		target = FindInnerMostTarget(target);
		if (target == null) {
			return;
		}

		_innerMember.SetValue(target, propertyValue);
	}

	public object GetValue(object target) {
		target = FindInnerMostTarget(target);

		if (target == null) {
			return null;
		}

		return _innerMember.GetValue(target);
	}

	public string FieldName {
		get { return _innerMember.FieldName; }
	}

	public Type PropertyType {
		get { return _innerMember.PropertyType; }
	}

	public Member InnerMember {
		get { return _innerMember.InnerMember; }
	}

	public IAccessor GetChildAccessor<T>(Expression<Func<T, object>> expression) {
		var member = expression.ToMember();
		var list = new List<Member>(_chain);
		list.Add(_innerMember.InnerMember);
		list.Add(member);

		return new PropertyChain(list.ToArray());
	}

	public string Name {
		get {
			string returnValue = string.Empty;
			foreach (var info in _chain) {
				returnValue += info.Name + ".";
			}

			returnValue += _innerMember.Name;

			return returnValue;
		}
	}

	#endregion

	private object FindInnerMostTarget(object target) {
		foreach (var info in _chain) {
			target = info.GetValue(target);
			if (target == null) {
				return null;
			}
		}

		return target;
	}
}
