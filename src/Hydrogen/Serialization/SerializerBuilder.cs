using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Hydrogen.FastReflection;
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
///				.Serialize(x => x.Property1, new Type1Serializer())
///				.Serialize(x => x.Property2, new Type2Serializer())
///				.Serialize(x => x.Property3, new Type3Serializer())
///				.Build();
///
///		var serializer = 
///			SerializerBuilder
///				.For&lt;TypeName&gt;()
///				.AllowNull()                   // allows top-level nulls to be serialized/deserialized
///				.SerializePublicProperties()   // uses default SerializerFactory to determine serializers for all public properties
///				.Ignore(x => x.SomePropertyIDontWantSerialized)
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

	public SerializerBuilder<TItem> Serialize<TMember>(Expression<Func<TItem, TMember>> memberExpression)
		=> Serialize(memberExpression, SerializerFactory.Default);


	public SerializerBuilder<TItem> Serialize<TMember>(Expression<Func<TItem, TMember>> memberExpression, SerializerFactory factory)
		=> Serialize(memberExpression, factory.GetSerializer<TMember>().AsSanitized());

	public SerializerBuilder<TItem> Serialize<TMember>(Expression<Func<TItem, TMember>> memberExpression, IItemSerializer<TMember> serializer) {
		Guard.ArgumentNotNull(memberExpression, nameof(memberExpression));
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		var member = memberExpression.ToMember();
		Guard.Argument(member.IsProperty || member.IsField, nameof(memberExpression), "Member must be a property or field");
		_memberBindings.Add(new(member, serializer));
		return this;
	}

	public SerializerBuilder<TItem> Ignore<TMember>(Expression<Func<TItem, TMember>> memberExpression) {
		Guard.ArgumentNotNull(memberExpression, nameof(memberExpression));
		var member = memberExpression.ToMember();
		int indexOf;
		while ((indexOf = _memberBindings.FindIndex(x => x.Member == member)) >= 0) {
			_memberBindings.RemoveAt(indexOf);
		}
		return this;
	}

	public SerializerBuilder<TItem> SerializeAllMembers() 
		=> SerializeAllMembers(SerializerFactory.Default);

	public SerializerBuilder<TItem> SerializeAllMembers(SerializerFactory factory) {
		
		foreach(var member in SerializerBuilder.GetSerializableMembers(typeof(TItem))) {
			if (factory.HasSerializer(member.PropertyType)) {
				_memberBindings.Add(new(member, factory.GetSerializer(member.PropertyType).AsSanitized()));
			} else {
				// No factory serializer available, build one dynamically using this method
				var memberSerializer = factory.Assemble(member.PropertyType);
				_memberBindings.Add(new(member, memberSerializer));
			}
		}

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


	public static IItemSerializer<T> AutoBuild<T>() 
		=> AutoBuild<T>(SerializerFactory.Default);

	public static IItemSerializer<T> AutoBuild<T>(SerializerFactory factory) 
		=> (IItemSerializer<T>)AutoBuild(typeof(T), factory);

	public static IItemSerializer AutoBuild(Type itemType, SerializerFactory factory) 
		=> factory.Assemble(itemType);
	

	public static Member[] GetSerializableMembers(Type type)
		=> type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
			.Where(x => x.CanRead && x.CanWrite)
			.Select(x => x.ToMember())
			.ToArray();
}
