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
///		SerializerBuilder.CreateFor&lt;Class&gt;()
///		.ForMember(x => x.Property1, new Type1Serializer())
///		.ForMember(x => x.Property2, new Type2Serializer())
///		.ForMember(x => x.Property3, new Type3Serializer())
///		.Build();
/// </remarks>
public class SerializerBuilder<TItem> {
	private readonly List<MemberSerializationBinding> _memberBindings;
	private Func<TItem> _activator;

	public SerializerBuilder() {
		_memberBindings = new List<MemberSerializationBinding>();
		_activator = null;
	}

	public SerializerBuilder<TItem> WithActivation(Func<TItem> activator) {
		_activator = activator;
		return this;
	}

	public SerializerBuilder<TItem> ForMember<TMember>(Expression<Func<TItem, TMember>> memberExpression, IItemSerializer<TMember> serializer) {
		Guard.ArgumentNotNull(memberExpression, nameof(memberExpression));
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		var member = memberExpression.ToMember();
		Guard.Argument(member.IsProperty || member.IsField, nameof(memberExpression), "Member must be a property or field");
		_memberBindings.Add(new (member, serializer.AsPacked()));
		return this;
	}

	public CompositeSerializer<TItem> Build() {
		Guard.Ensure(_activator is not null, "Activation has not been set");
		return new CompositeSerializer<TItem>(_activator, _memberBindings.ToArray());
	}

}

/// <summary>
/// Builds a serializer for a given type by allowing the client to determine individual member serializers.
/// </summary>
public static class SerializerBuilder {
	public static SerializerBuilder<TItem> CreateFor<TItem>() where TItem : new()
		=> CreateFor(() => new TItem());

	public static SerializerBuilder<TItem> CreateFor<TItem>(Func<TItem> activator)
		=> new SerializerBuilder<TItem>().WithActivation(activator);
}
