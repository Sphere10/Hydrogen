using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Hydrogen.Mapping;

namespace Hydrogen;

/// <summary>
/// Builds a serializer for a given type by allowing the client to determine individual member serializers.
/// </summary>
/// <typeparam name="TItem">The type being built.</typeparam>
/// <remarks>
///		var serializer = 
///			SerializerBuilder
///				.For&lt;TypeName&gt;()
///				.ForMember(x => x.Property1, new Type1Serializer())
///				.ForMember(x => x.Property2, new Type2Serializer())
///				.ForMember(x => x.Property3, new Type3Serializer())
///				.Build();
/// </remarks>
public class SerializerBuilder<TItem> {
	private readonly List<MemberSerializationBinding> _memberBindings;
	private Func<TItem> _activator;
	private bool _allowNull;

	internal SerializerBuilder() {
		_memberBindings = new List<MemberSerializationBinding>();
		_activator = null;
		_allowNull = false;
	}

	public SerializerBuilder<TItem> WithActivation(Func<TItem> activator) {
		_activator = activator;
		return this;
	}

	public SerializerBuilder<TItem> AllowNull() {
		_allowNull = true;
		return this;
	}

	public SerializerBuilder<TItem> ForMember<TMember>(Expression<Func<TItem, TMember>> memberExpression, IItemSerializer<TMember> serializer) {
		Guard.ArgumentNotNull(memberExpression, nameof(memberExpression));
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		var member = memberExpression.ToMember();
		Guard.Argument(member.IsProperty || member.IsField, nameof(memberExpression), "Member must be a property or field");
		_memberBindings.Add(new (member, serializer));
		return this;
	}

	public IItemSerializer<TItem> Build() {
		Guard.Ensure(_activator is not null, "Activation has not been set");
		IItemSerializer<TItem> serializer = new CompositeSerializer<TItem>(_activator, _memberBindings.ToArray());
		if (_allowNull)
			serializer = serializer.AsNullable();
		return serializer;
	}

}

/// <summary>
/// Builds a serializer for a given type by allowing the client to determine individual member serializers.
/// </summary>
public static class SerializerBuilder {
	public static SerializerBuilder<TItem> For<TItem>() where TItem : new()
		=> For(() => new TItem());

	public static SerializerBuilder<TItem> For<TItem>(Func<TItem> activator)
		=> new SerializerBuilder<TItem>().WithActivation(activator);

}
