using System;
using System.Collections;
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
public class SerializerBuilder<TItem> : SerializerBuilder {
	
	public SerializerBuilder() 
		: base(typeof(TItem)) {
	}

	public SerializerBuilder<TItem> WithActivation(Func<TItem> activator) 
		=> (SerializerBuilder<TItem>)WithActivation((Delegate)activator);

	public new SerializerBuilder<TItem> UseFactory(SerializerFactory factory)
		=> (SerializerBuilder<TItem>)base.UseFactory(factory);

	public SerializerBuilder<TItem> Serialize<TMember>(Expression<Func<TItem, TMember>> memberExpression)
		=> Serialize(memberExpression, SerializerFactory.Default);

	public SerializerBuilder<TItem> Serialize<TMember>(Expression<Func<TItem, TMember>> memberExpression, SerializerFactory factory)
		=> Serialize(memberExpression, factory.GetSerializer<TMember>());

	public SerializerBuilder<TItem> Serialize<TMember>(Expression<Func<TItem, TMember>> memberExpression, IItemSerializer<TMember> serializer) 
		=> Serialize(memberExpression.ToMember(), serializer);

	public new SerializerBuilder<TItem> Serialize(Member member, IItemSerializer serializer)
		=> (SerializerBuilder<TItem>)base.Serialize(member, serializer);

	public new SerializerBuilder<TItem> Ignore(Member member)
		=> (SerializerBuilder<TItem>)base.Ignore(member);

	public SerializerBuilder<TItem> Ignore<TMember>(Expression<Func<TItem, TMember>> memberExpression) 
		=> (SerializerBuilder<TItem>)Ignore(memberExpression.ToMember());

	public new SerializerBuilder<TItem> SerializeMembersUsingExistingSerializers()
		=> (SerializerBuilder<TItem>)base.SerializeMembersUsingExistingSerializers();

	public new SerializerBuilder<TItem> SerializeMembersAutomatically(bool retainMemberSerializersAsRegistrations = true)
		=> (SerializerBuilder<TItem>)base.SerializeMembersAutomatically(retainMemberSerializersAsRegistrations);

	public new SerializerBuilder<TItem> SerializeMembers(Func<SerializerFactory, Member, IItemSerializer> memberSerializerBuilder, bool retainMemberSerializersAsRegistrations = true)
		=> (SerializerBuilder<TItem>)base.SerializeMembers(memberSerializerBuilder, retainMemberSerializersAsRegistrations);

	public new IItemSerializer<TItem> Build() 
		=> (IItemSerializer<TItem>)base.Build();

}

/// <summary>
/// Builds a serializer for a given type by allowing the client to determine individual member serializers.
/// </summary>
public abstract class SerializerBuilder {
	private readonly List<MemberSerializationBinding> _memberBindings;
	private IItemSerializer _compositeSerializer;
	private SerializerFactory _factory;
	private Delegate _activator;
	//private ReferenceSerializerMode _referenceMode;

	protected SerializerBuilder(Type itemType) {
		ItemType = itemType;
		
		_compositeSerializer = CompositeSerializer.Create(ItemType);
		_memberBindings = new List<MemberSerializationBinding>();
		//_allowNull = false;
		//_referenceMode = ReferenceSerializerMode.Default;
	}

	public Type ItemType { get; }

	//public virtual SerializerBuilder AllowNull(bool value) {
	//	_referenceMode = value ? _referenceMode | ReferenceSerializerMode.SupportNull : _referenceMode & ~ReferenceSerializerMode.SupportNull;
	//	return this;
	//}

	//public virtual SerializerBuilder AllowContextReferences(bool value) {
	//	_referenceMode = value ? _referenceMode | ReferenceSerializerMode.SupportContextReferences : _referenceMode & ~ReferenceSerializerMode.SupportContextReferences;
	//	return this;
	//}

	//public virtual SerializerBuilder AllowExternalReference(bool value) {
	//	_referenceMode = value ? _referenceMode | ReferenceSerializerMode.SupportExternalReferences : _referenceMode & ~ReferenceSerializerMode.SupportExternalReferences;
	//	return this;
	//}

	public SerializerBuilder WithActivation(Delegate activator) {
		_activator = activator;
		return this;
	}

	public virtual SerializerBuilder UseFactory(SerializerFactory factory) {
		Guard.Argument(!_factory.ContainsSerializer(ItemType), nameof(factory), "Serializer already registered for type");
		_factory = factory;
		return this;
	}

	public virtual SerializerBuilder Serialize(Member member, IItemSerializer serializer) {
		Guard.ArgumentNotNull(member, nameof(member));
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		Guard.Argument(member.DeclaringType == ItemType, nameof(member), "Member must be declared on the item type");
		Guard.Argument(member.IsProperty || member.IsField, nameof(member), "Member must be a property or field");
		_memberBindings.Add(new(member, serializer));
		return this;
	}

	public SerializerBuilder Ignore(Member member) {
		Guard.ArgumentNotNull(member, nameof(member));
		int indexOf;
		while ((indexOf = _memberBindings.FindIndex(x => x.Member == member)) >= 0) {
			_memberBindings.RemoveAt(indexOf);
		}
		return this;
	}

	public virtual SerializerBuilder SerializeMembersUsingExistingSerializers() 
		=> SerializeMembers((_, member) => throw new InvalidOperationException($"No serializer found for member type {member.PropertyType} of member {member.Name}"));
			
	public virtual SerializerBuilder SerializeMembersAutomatically(bool retainMemberSerializersAsRegistrations = true) 
		=> SerializeMembers((factory, member) => FactoryAssemble(factory, member.PropertyType, retainMemberSerializersAsRegistrations), retainMemberSerializersAsRegistrations);

	public virtual SerializerBuilder SerializeMembers(Func<SerializerFactory, Member, IItemSerializer> memberSerializerBuilder, bool retainMemberSerializersAsRegistrations = true) {
		Guard.Ensure(_factory is not null, "Factory is required for automatic member serialization");
		Guard.Ensure(!_factory.ContainsSerializer(ItemType), "Serializer already registered for type");

		// If not retaining assembled serialized in the factory, we use an anonymous chained factory to avoid polluting the factory
		var typeCodeStart = _factory.MaxGeneratedTypeCode + 1;
		var factory = retainMemberSerializersAsRegistrations ? _factory : new SerializerFactory(_factory) { MinGeneratedTypeCode = typeCodeStart };

		var membersWithoutBindings = GetSerializableMembers(ItemType).Except(_memberBindings.Select(x => x.Member)).ToArray();
		foreach (var member in membersWithoutBindings) {
			var referenceMode =  ReferenceModeAttribute.GetReferenceModeOrDefault(member.MemberInfo); 
			if (!_factory.TryGetSerializer(member.PropertyType, referenceMode, out var memberSerializer, out _)) {
				memberSerializerBuilder.Invoke(_factory, member);
			}
			_memberBindings.Add(new(member, memberSerializer));
		}

		if (!retainMemberSerializersAsRegistrations)
			factory.FinishRegistrations();

		return this;
	}

	public IItemSerializer Build() {
		Guard.Ensure(_activator is not null, "An activator is required");
		CompositeSerializer.Configure(_compositeSerializer, _activator, _memberBindings.ToArray());
		if (_factory is not null && !_factory.ContainsSerializer(ItemType))
			_factory.Register(ItemType, _compositeSerializer);
		return _compositeSerializer;
	}

	public static SerializerBuilder<TItem> For<TItem>() where TItem : new()
		=> For(() => new TItem());

	public static SerializerBuilder<TItem> For<TItem>(Func<TItem> activator)
		=> new SerializerBuilder<TItem>().WithActivation(activator);

	public static IItemSerializer<T> FactoryAssemble<T>() 
		=> FactoryAssemble<T>(SerializerFactory.Default);

	public static IItemSerializer<T> FactoryAssemble<T>(SerializerFactory factory) 
		=> (IItemSerializer<T>)FactoryAssemble(typeof(T), factory);

	public static IItemSerializer FactoryAssemble(Type itemType, SerializerFactory factory) 
		=> FactoryAssemble(factory, itemType, false, factory.MaxGeneratedTypeCode + 1);

	public static IItemSerializer FactoryAssemble(SerializerFactory factory, Type itemType, bool retainAssembledTypeSerializersInFactory, long? typeCodeStart = null) {

		// If not retaining assembled serialized in the factory, we use an anonymous chained factory to avoid polluting the factory
		typeCodeStart ??= factory.MaxGeneratedTypeCode + 1;
		factory = retainAssembledTypeSerializersInFactory ? factory : new SerializerFactory(factory) { MinGeneratedTypeCode = typeCodeStart.Value };

		var assembledSerializer = BuildRecursively(itemType, ReferenceSerializerMode.Default, new());

		if (!retainAssembledTypeSerializersInFactory)
			factory.FinishRegistrations();

		return assembledSerializer;

		IItemSerializer BuildRecursively(Type requestedType, ReferenceSerializerMode mode, HashSet<Type> visitedTypes) {
			visitedTypes.Add(requestedType);

			// If a serializer exists for type, use it
			if (factory.TryGetSerializer(requestedType, mode, out var serializer, out var missingSerializers)) {
				return serializer;
			}

			// include the generic arguments of missing serializers as missing serializers themselves
			missingSerializers.AddRange(
				missingSerializers
					.Where(x => x.IsConstructedGenericType)
					.SelectMany(x => x.GetGenericArgumentsTransitively().Where(s => !factory.ContainsSerializer(s)))
					.Except(visitedTypes)
					.ToArray()
			);

			// Add any known sub-types marked on this type
			KnownSubTypeAttribute
				.GetKnownSubTypes(requestedType)
				.Except(visitedTypes)
				.Where(x => !factory.ContainsSerializer(x))
				.ForEach(x => missingSerializers.Add(x));

			// TODO: ensure you don't add the same type to missingSerializers multiple times (infinite recusrion)

			// No serializer found, make sure we build dependent serializers first
			foreach (var missing in missingSerializers.Except(requestedType))
				BuildRecursively(missing, ReferenceSerializerMode.Default, visitedTypes);
			
			// Build serializer for this type
			if (requestedType.IsEnumOrNullableEnum(out var enumType)) {
				// Case for Enums
				factory.RegisterEnum(enumType);
			} else if (requestedType.IsAbstract) {
				// Case for abstract types
				// NOTE: concrete subtypes can be registered via KnownSubTypeAttribute annotations scanned above
				var polymorphicSerializer = PolymorphicSerializer.Create(factory, requestedType);
				factory.RegisterInternal(factory.GenerateTypeCode(), requestedType, polymorphicSerializer.GetType(), polymorphicSerializer, null);
			} else if (missingSerializers.Contains(requestedType)) {
				// Missing a serializer for requested type, so build it as a CompositeSerializer
				var compositeSerializer = CompositeSerializer.Create(requestedType);
				factory.RegisterInternal(factory.GenerateTypeCode(), requestedType, compositeSerializer.GetType(), compositeSerializer, null);
				var members = GetSerializableMembers(requestedType);
				var memberBindings = new List<MemberSerializationBinding>(members.Length);
				foreach (var member in members) {
					var referenceMode =  ReferenceModeAttribute.GetReferenceModeOrDefault(member.MemberInfo); 
					var memberSerializer = BuildRecursively(member.PropertyType, referenceMode, visitedTypes);
					memberBindings.Add(new(member, memberSerializer));
				}
				CompositeSerializer.Configure(compositeSerializer, requestedType, memberBindings);
			} 

			// serializer now registered, return it
			return factory.GetSerializer(requestedType, mode);
		}
	}

	public static Member[] GetSerializableMembers(Type type) {
		var inheritanceDepth = type.Visit(x => x.BaseType, x => x is not null).ToList();
		return type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
			.Where(x => x is PropertyInfo || x is FieldInfo)
			.Select(x => x.ToMember())
			.Where(x => x.CanRead && x.CanWrite)
			.Where(x => !x.MemberInfo.HasAttribute<TransientAttribute>(false))
			.OrderByDescending(x => inheritanceDepth.IndexOf(x.DeclaringType))  // this order to ensure base-type members are serialized before sub-type members
			.ToArray();
	}

}
