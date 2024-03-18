// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Hydrogen;

[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
public class SerializerFactory {
	internal const int PermanentTypeCodeStartDefault = 10; 
	internal const int EphemeralTypeCodeStartDefault = 65536; // Type codes for ephemerally registered types start here. This allows you to update your core serializer registrations over time without affecting previous serialized objects.

	private IDictionary<long, Registration> _registrations;
	private BijectiveDictionary<Type, long> _registrationsByType;
	private readonly ICache<Type, RecursiveDataType<long>> _getSerializerHierarchyCache;
	private readonly ICache<RecursiveDataType<long>, IItemSerializer> _fromSerializerHierarchyCache;
	private readonly ICache<Type, IItemSerializer> _getSerializerCache;
	private SerializerFactory _nextFactory;
	private bool _readOnly;

	static SerializerFactory() {
		Default = new SerializerFactory();
		RegisterDefaults(Default);
		Default._readOnly = true;
	}

	public SerializerFactory() {
		_registrations = new Dictionary<long, Registration>();
		_registrationsByType = new BijectiveDictionary<Type, long>(new TypeEquivalenceComparer(), EqualityComparer<long>.Default);
		_getSerializerHierarchyCache = new ActionCache<Type, RecursiveDataType<long>>(GetSerializerHierarchyInternal, keyComparer: TypeEquivalenceComparer.Instance);
		_fromSerializerHierarchyCache = new ActionCache<RecursiveDataType<long>, IItemSerializer>(FromSerializerHierarchyInternal);
		_getSerializerCache = new ActionCache<Type, IItemSerializer>(GetCachedSerializer_Fetch,  keyComparer: TypeEquivalenceComparer.Instance);
		_readOnly = false;
		_nextFactory = null;
	}

	public SerializerFactory(SerializerFactory baseFactory) : this() {
		CopyRegistrations(baseFactory);
	}

	public IEnumerable<Type> RegisteredTypes => _registrationsByType.Keys;

	public static SerializerFactory Default { get; }

	private long MinimumGeneratedTypeCode { get; init; } = PermanentTypeCodeStartDefault;

	public static void RegisterDefaults(SerializerFactory factory) {
		// register self-assembling factory for object
		
		// fundamental serialization types
		factory.Register<object, PureObjectSerializer>();
		factory.Register(typeof(Array), typeof(ArraySerializer<>));  // special general array serializer
		factory.Register(typeof (Nullable<>), typeof (NullableSerializer<>)); // nullable struct serializer

		// primitive & base .net types
		factory.Register(PrimitiveSerializer<bool>.Instance);
		factory.Register(PrimitiveSerializer<sbyte>.Instance);
		factory.Register(PrimitiveSerializer<byte>.Instance);
		factory.Register(PrimitiveSerializer<char>.Instance);
		factory.Register(PrimitiveSerializer<ushort>.Instance);
		factory.Register(PrimitiveSerializer<short>.Instance);
		factory.Register(PrimitiveSerializer<uint>.Instance);
		factory.Register(PrimitiveSerializer<int>.Instance);
		factory.Register(PrimitiveSerializer<ulong>.Instance);
		factory.Register(PrimitiveSerializer<long>.Instance);
		factory.Register(PrimitiveSerializer<float>.Instance);
		factory.Register(PrimitiveSerializer<double>.Instance);
		factory.Register(PrimitiveSerializer<decimal>.Instance);
		factory.Register(DateTimeSerializer.Instance);
		factory.Register(TimeSpanSerializer.Instance);
		factory.Register(DateTimeOffsetSerializer.Instance);
		factory.Register(GuidSerializer.Instance);

		// their nullables
		factory.Register(NullableSerializer<bool>.Instance);
		factory.Register(NullableSerializer<byte>.Instance);
		factory.Register(NullableSerializer<char>.Instance);
		factory.Register(NullableSerializer<ushort>.Instance);
		factory.Register(NullableSerializer<short>.Instance);
		factory.Register(NullableSerializer<uint>.Instance);
		factory.Register(NullableSerializer<int>.Instance);
		factory.Register(NullableSerializer<ulong>.Instance);
		factory.Register(NullableSerializer<long>.Instance);
		factory.Register(NullableSerializer<float>.Instance);
		factory.Register(NullableSerializer<double>.Instance);
		factory.Register(NullableSerializer<decimal>.Instance);
		factory.Register(new NullableSerializer<DateTime>(DateTimeSerializer.Instance));
		factory.Register(new NullableSerializer<TimeSpan>(TimeSpanSerializer.Instance));
		factory.Register(new NullableSerializer<DateTimeOffset>(DateTimeOffsetSerializer.Instance));
		factory.Register(new NullableSerializer<Guid>(GuidSerializer.Instance));

		// other base .net types
		factory.Register(new StringSerializer());
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>));

		// general collections
		factory.Register<ArrayList, ArrayListSerializer>(() => new ArrayListSerializer(factory));
		factory.Register(ByteArraySerializer.Instance);
		factory.Register(typeof(IEnumerable<>), typeof(EnumerableSerializer<>));
		factory.Register(typeof(ICollection<>), typeof(CollectionInterfaceSerializer<>));
		factory.Register(typeof(Collection<>), typeof(CollectionSerializer<>));
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>));
		factory.Register(typeof(List<>), typeof(ListSerializer<>));
		factory.Register(typeof(IDictionary<,>), typeof(DictionaryInterfaceSerializer<,>));
		factory.Register(typeof(Dictionary<,>), typeof(DictionarySerializer<,>));
		factory.Register(typeof(ISet<>), typeof(SetInterfaceSerializer<>));
		factory.Register(typeof(HashSet<>), typeof(HashSetSerializer<>));
		factory.Register(typeof(SortedSet<>), typeof(SortedSetSerializer<>));
	}

	public void CopyRegistrations(SerializerFactory factory) {
		Guard.ArgumentNotNull(factory, nameof(factory));
		Guard.Ensure(!_readOnly, "Factory is read-only and cannot be changed");

		foreach (var registration in factory._registrations) {
			var newRegistration = new Registration {
				TypeCode = registration.Value.TypeCode,
				DataType = registration.Value.DataType,
				SerializerType = registration.Value.SerializerType,
				Factory = registration.Value.Factory,
				SerializerFactory = this    // when copying another factory, we set the new factory as the owner of the registration
			};
			_registrations.Add(registration.Key, newRegistration);
			_registrationsByType.Add(registration.Value.DataType, registration.Key);
		}
	}

	#region Register

	public void RegisterEnum<TEnum>() where TEnum : struct, Enum 
		=> RegisterEnum(typeof(TEnum));

	public void RegisterEnum(Type enumType) {
		Guard.ArgumentNotNull(enumType, nameof(enumType));
		Guard.Argument(enumType.IsEnum, nameof(enumType), $"Type {enumType.Name} is not an enum");
		var enumSerializer = (IItemSerializer) typeof(EnumSerializer<>).MakeGenericType(enumType).ActivateWithCompatibleArgs();
		var nullableEnumSerializer = (IItemSerializer) typeof(NullableSerializer<>).MakeGenericType(enumType).ActivateWithCompatibleArgs(enumSerializer);
		RegisterInternal(GenerateTypeCode(), enumType, enumSerializer.GetType(), enumSerializer, null);
		RegisterInternal(GenerateTypeCode(), typeof(Nullable<>).MakeGenericType(enumType), nullableEnumSerializer.GetType(), nullableEnumSerializer, null);
	}

	public void Register<TItem>(IItemSerializer<TItem> serializerInstance)
		=> RegisterInternal(GenerateTypeCode(), typeof(TItem), serializerInstance.GetType(), serializerInstance, null);

	public void Register<TItem, TSerializer>() where TSerializer : IItemSerializer<TItem>, new()
		=> RegisterInternal(GenerateTypeCode(), typeof(TItem), typeof(TSerializer), null, (_,_) => new TSerializer());

	public void Register<TItem, TSerializer>(Func<TSerializer> factory) 
		=> RegisterInternal(GenerateTypeCode(), typeof(TItem), typeof(TSerializer), null, (_,_) => (IItemSerializer)factory());

	public void Register<TItem>(Type serializerType) 
		=> RegisterInternal(GenerateTypeCode(), typeof(TItem), serializerType, null, null);

	public void Register(Type dataType, Type serializerType) 
		=> RegisterInternal(GenerateTypeCode(), dataType, serializerType, null, null);

	public void RegisterAutoBuild<T>() 
		=> RegisterAutoBuild(typeof(T));

	public void RegisterAutoBuild(Type dataType) 
		=> AssembleSerializer(this, dataType, true, MinimumGeneratedTypeCode);

	private void RegisterInternal(long typeCode, Type dataType, Type serializerType, IItemSerializer serializerInstance, Func<Registration, Type, IItemSerializer> factory) {
		Guard.ArgumentNotNull(dataType, nameof(dataType));
		Guard.ArgumentNotNull(serializerType, nameof(serializerType));
		//Guard.Against(dataType == typeof(object), $"Cannot register serializer for {nameof(Object)} type");
		Guard.Ensure(!_readOnly, "Factory is read-only and cannot be changed");

		if (dataType == typeof(Array)) {
			// Special registration for array serialization
			Guard.Ensure(serializerType.IsPartialOrGenericTypeDefinition(), $"Serializer type {serializerType.Name} must be a generic type definition for data type {dataType.Name}");
			Guard.Argument(serializerType.IsSubtypeOfGenericType(typeof(IItemSerializer<>), out var matchedGenericType) &&  matchedGenericType.ContainsGenericParameters && matchedGenericType.GenericTypeArguments[0].IsArray, nameof(serializerType), $"Serializer type must implement IItemSerializer<T[]>");
		} else if (dataType.IsFullyConstructed() || serializerType.IsActivatable()) {
			Guard.Ensure(dataType.IsFullyConstructed(), $"Data type {dataType.Name} must be a fully constructed type for serializer type ${serializerType.Name}");
			Guard.Ensure(serializerType.IsActivatable(), $"Serializer type {serializerType.Name} must be an instantiable type for serializer type {dataType.Name}");
			Guard.Argument(serializerType.IsSubTypeOf(typeof(IItemSerializer<>).MakeGenericType(dataType)), nameof(serializerType), $"Serializer type must implement IItemSerializer<${dataType.Name}>");		
		} else {
			Guard.Ensure(dataType.IsPartialOrGenericTypeDefinition(), $"Data type {dataType.Name} must be a generic type definition for serializer type {serializerType.Name}");
			Guard.Ensure(serializerType.IsPartialOrGenericTypeDefinition(), $"Serializer type {serializerType.Name} must be a generic type definition for data type {dataType.Name}");
			Guard.Argument(serializerType.IsSubtypeOfGenericType(typeof(IItemSerializer<>).MakeGenericType(dataType)), nameof(serializerType), $"Serializer type must implement IItemSerializer<${dataType.Name}>");
		}

		// ensure type-code is not already registered
		if (_registrations.ContainsKey(typeCode))
			throw new InvalidOperationException($"Type code {typeCode} is already used");

		// ensure registration is not already registered
		if (_registrationsByType.ContainsKey(dataType))
			throw new InvalidOperationException($"A serializer for type {dataType.Name} has already been registered");

		// ensure dataType's generic type arguments have a serializer if the serializer is also an open generic type
		if (dataType.IsConstructedGenericType && !serializerType.IsConstructedGenericType) {
			foreach (var genericType in dataType.GetGenericArguments())
				if (!_registrationsByType.ContainsKey(genericType))
					throw new InvalidOperationException($"A serializer for generic type argument '{genericType.Name}' is required before registering the open generic serializer for '{serializerType.Name}'");
		}

		// add registration (we fix up object factory after, since it requires a reference to the registration)
		var registration = new Registration {
			TypeCode = typeCode,
			DataType = dataType,
			SerializerType = serializerType,
			Factory = null,
			SerializerFactory = this
		};

		// Ensure instance is of correct type
		if (serializerInstance != null) {
			if (!serializerType.IsInstanceOfType(serializerInstance))
				throw new InvalidOperationException($"Serializer instance is not of type {serializerType.Name}");

			Guard.Ensure(factory is null, "Factory must be null if serializer instance is provided");
			// serializerInstance = serializerInstance.AsReferenceSerializer(); // ensure serializer instance is reference serializer xxxxxxxxxxxxxxxxxxxxx
			factory =  (_, _) => serializerInstance;
		} else if (factory is null) {
			// get all sub-types
			factory = (serializerFactory, requested) => CreateSerializerInstance(serializerFactory, requested, dataType, serializerType);
		} 

		registration.Factory = factory;
		_registrations.Add(typeCode, registration);
		_registrationsByType.Add(dataType, typeCode);

		_getSerializerCache.Purge();
		_fromSerializerHierarchyCache.Purge();
	}

	#endregion

	#region Has Serializer

	public bool HasSerializer(Type dataType) => TryFindCompatibleSerializer(dataType, out _, out _);

	#endregion

	#region GetSerializer

	public IItemSerializer<T> GetSerializer<T>() 
		=> GetSerializer<T>(EphemeralTypeCodeStartDefault);

	public IItemSerializer<T> GetSerializer<T>(long typeCodeStart) 
		=> (IItemSerializer<T>)AssembleSerializer(this, typeof(T), false, typeCodeStart);

	public IItemSerializer GetSerializer(Type type) 
		=> GetSerializer(type, EphemeralTypeCodeStartDefault);

	public IItemSerializer GetSerializer(Type type, long typeCodeStart) 
		=> AssembleSerializer(this, type, false, typeCodeStart);

	#endregion

	#region GetRegisteredSerializer

	public IItemSerializer<TType> GetRegisteredSerializer<TType>(bool referenceSupport = true) 
		=> GetRegisteredSerializer<TType>(typeof(TType));

	public IItemSerializer<TBase> GetRegisteredSerializer<TBase, TConcrete>(bool referenceSupport = true) where TConcrete : TBase 
		=> GetRegisteredSerializer<TBase>(typeof(TConcrete));
	
	public IItemSerializer<TSerializerDataType> GetRegisteredSerializer<TSerializerDataType>(Type dataType, bool referenceSupport = true) {
		Guard.ArgumentNotNull(dataType, nameof(dataType));
		var serializerDataType = typeof(TSerializerDataType);
		Guard.Argument(dataType.IsAssignableTo(serializerDataType), nameof(dataType), $"{dataType.ToStringCS()} must be assignable to {serializerDataType.ToStringCS()}");
		var serializerObj = GetRegisteredSerializer(dataType, referenceSupport);
		var serializer = serializerObj as IItemSerializer<TSerializerDataType>;
		if (serializer is null) {
			serializer = new CastedSerializer<TSerializerDataType>(serializerObj);
		}
		return serializer;
	}

	public IItemSerializer GetRegisteredSerializer(Type dataType, bool referenceSupport = true) {
		Guard.ArgumentNotNull(dataType, nameof(dataType));
		var serializer = GetCachedSerializer(dataType);
		serializer = referenceSupport ? serializer.AsReferenceSerializer() : serializer.AsDereferencedSerializer();
		return serializer;
	}

	#endregion

	#region Serializer hierarchy methods

	public RecursiveDataType<long> GetSerializerHierarchy(Type dataType) 
		=> _getSerializerHierarchyCache[dataType];

	public IItemSerializer FromSerializerHierarchy(RecursiveDataType<long> serializerHierarchy) 
		=> _fromSerializerHierarchyCache[serializerHierarchy];

	public long CountSubSerializers(long typeCode) {
		if (!_registrations.TryGetValue(typeCode, out var registration))
			throw new InvalidOperationException($"No serializer registered for type code {typeCode}");
		return registration.SerializerType.IsGenericTypeDefinition ? registration.SerializerType.GetGenericArguments().Length : 0;
	}

	#endregion

	#region Aux

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private IItemSerializer GetCachedSerializer(Type dataType) => _getSerializerCache[dataType];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private IItemSerializer GetCachedSerializer_Fetch(Type dataType) {
		Guard.ArgumentNotNull(dataType, nameof(dataType));
		var registration = FindCompatibleSerializer(dataType, out _);
		return registration.Factory(registration, dataType);
	}

	private bool TryFindCompatibleSerializer(Type dataType, out long typeCode, out Registration registration) {
		// Try direct lookup
		if (_registrationsByType.TryGetValue(dataType, out typeCode)) {
			registration = _registrations[typeCode];
			return true;
		}

		// Special case for arrays
		if (dataType.IsArray && _registrationsByType.TryGetValue(typeof(Array), out typeCode)) {
			registration = _registrations[typeCode];
			var hasArrayItemTypeSerializer = TryFindCompatibleSerializer(dataType.GetElementType(), out _, out _);
			return hasArrayItemTypeSerializer;
		}

		// Try generic lookup
		if (dataType.IsGenericType && _registrationsByType.TryGetValue(dataType.GetGenericTypeDefinition(), out typeCode)) {
			registration = _registrations[typeCode];
			var hasGenericArgumentsSerializers = dataType.GetGenericArguments().All(x => TryFindCompatibleSerializer(x, out _, out _));
			return hasGenericArgumentsSerializers;
		}
	
		// No serializer found
		typeCode = -1;
		registration = default;
		return false;
	}

	private Registration FindCompatibleSerializer(Type dataType, out long typeCode) {
		if (!TryFindCompatibleSerializer(dataType, out typeCode, out var registration))
			throw new InvalidOperationException($"No serializer registered for type {dataType.Name}");
		return registration;
	}

	private Registration GetRegistration(long typeCode) {
		if (!_registrations.TryGetValue(typeCode, out var registration))
			throw new InvalidOperationException($"No serializer registered for type code {typeCode}");
		return registration;
	}

	private long GenerateTypeCode() => Tools.Values.Max( _registrations.Count > 0 ? _registrations.Keys.Max() + 1 : MinimumGeneratedTypeCode, MinimumGeneratedTypeCode);

	private static IItemSerializer CreateSerializerInstance(Registration registration, Type requestedDataType, Type registeredDataType, Type registeredSerializerType) {
		Guard.Argument(!requestedDataType.IsGenericTypeDefinition, nameof(requestedDataType), $"Requested data type {requestedDataType.Name} cannot be a generic type definition");
		if (registeredDataType.IsGenericTypeDefinition)
			Guard.Ensure(requestedDataType.IsConstructedGenericTypeOf(registeredDataType), $"Constructed type {requestedDataType.Name} is not a constructed generic type of {registeredDataType.Name}");

		var subTypes = requestedDataType switch {
			{ IsArray: true } => new [] { requestedDataType.GetElementType() },
			{ IsConstructedGenericType: true } => requestedDataType.GetGenericArguments(),
		};

		var subTypeSerializers = requestedDataType switch {
			_ => subTypes.Select(registration.SerializerFactory.GetCachedSerializer).ToArray()
		};
		var serializerType = registeredSerializerType;
		if (serializerType.IsGenericTypeDefinition)
			serializerType = serializerType.MakeGenericType(subTypes);
		return (IItemSerializer) TypeActivator.ActivateWithCompatibleArgs(serializerType, subTypeSerializers);
	}
		
	private Type ConstructType(RecursiveDataType<long> serializerHierarchy) {
		var registration = GetRegistration(serializerHierarchy.State);
		var dataType = registration.DataType;
		if (!dataType.IsGenericTypeDefinition && dataType != typeof(Array))
			return dataType;
		var subTypes = serializerHierarchy.SubStates.Select(ConstructType).ToArray();
		
		return dataType switch {
			_ when dataType == typeof(Array) => subTypes[0].MakeArrayType(),
			_ => dataType.MakeGenericType(subTypes)
		};
	}

	private RecursiveDataType<long> GetSerializerHierarchyInternal(Type type) {
		Guard.ArgumentNotNull(type, nameof(type));
		Guard.Argument(!type.IsGenericTypeDefinition, nameof(type), $"Type {type.ToStringCS()} must not be a generic type definition");
		long typeCode;
		Registration serializerRegistration;
		RecursiveDataType<long>[] childSerializers;
	
		serializerRegistration = FindCompatibleSerializer(type, out typeCode);
		if (serializerRegistration.DataType == typeof(Array)) {
			// arrays are special case, we matched generic array serializer, so sub-serializers are for element type
			childSerializers = new[] { GetSerializerHierarchy(type.GetElementType()) };
		} else if (serializerRegistration.DataType == typeof(Enum)) { 
			// enums are special case, we matched enum serializer, so sub-serializers are for enum type
			childSerializers = new[] { GetSerializerHierarchy(type.GetEnumUnderlyingType()) };
		} else {
			// case (2), now get any sub-serializers required for generic serializer
			childSerializers =
				serializerRegistration.SerializerType.IsGenericTypeDefinition ?
					type.GetGenericArguments().Select(GetSerializerHierarchy).ToArray() :
					Enumerable.Empty<RecursiveDataType<long>>().ToArray();
		}

		return new RecursiveDataType<long>(typeCode, childSerializers);
	}

	private IItemSerializer FromSerializerHierarchyInternal(RecursiveDataType<long> serializerHierarchy) {
		var registration = GetRegistration(serializerHierarchy.State);
		var type = ConstructType(serializerHierarchy);
		return registration.Factory(registration, type);
	}

	private static IItemSerializer AssembleSerializer(SerializerFactory serializerFactory, Type itemType, bool retainRegisteredTypesInFactory, long typeCodeStart) {
		
		// During the construction, a factory is required to store generated serializers.
		var factoryToUse = retainRegisteredTypesInFactory ? serializerFactory : new SerializerFactory(serializerFactory) { MinimumGeneratedTypeCode = typeCodeStart };

		var assembledSerializer = AssembleRecursively(factoryToUse, itemType);

		return assembledSerializer;

		// TODO: support nested-types by intelligently tracking parent 
		IItemSerializer AssembleRecursively(SerializerFactory factory, Type itemType) {

			// Ensure serializers for component types are registered
			// (i.e. resolving serializer for List<UnregisteredType> serializer requires a serializer for UnregisteredType)
			foreach (var genericType in GetUnregisteredComponentTypes(factory, itemType))
				AssembleRecursively(factory, genericType);

			// If serializer already exists for this type in factory, use that
			if (factory.HasSerializer(itemType))
				return factory.GetCachedSerializer(itemType);

			// Special Case: if we're serializing an enum (or nullable enum), we register it with the factory now and return
			if (itemType.IsEnum || itemType.IsConstructedGenericTypeOf(typeof(Nullable<>)) && itemType.GenericTypeArguments[0].IsEnum) {
				factory.RegisterEnum(itemType.IsEnum ? itemType : itemType.GenericTypeArguments[0]);
				return factory.GetCachedSerializer(itemType);
			}

			// No serializer registered so we need to assemble one as a CompositeSerializer. First, we need to 
			// register the serializer (before it is assembled) as it may recursively refer to itself. So we 
			// activate a CompositeSerializer with no members (we'll configure it later)
			var compositeSerializer = 
				(IItemSerializer) typeof(CompositeSerializer<>)
				.MakeGenericType(itemType)
				.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)
				.Invoke(null);

			var serializer = 
				itemType.IsValueType ? 
				compositeSerializer : 
				(IItemSerializer)typeof(ReferenceSerializer<>).MakeGenericType(itemType).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null,  new Type[] { typeof(IItemSerializer<>).MakeGenericType(itemType) }, null).Invoke(new object[] { compositeSerializer });


			// register serializer instance now as it may be re-used in component serializers (recursive types)
			if (itemType != typeof(object))
				factory.RegisterInternal(factory.GenerateTypeCode(), itemType, compositeSerializer.GetType(), compositeSerializer, null);

			// Create the member serializers
			var members = SerializerBuilder.GetSerializableMembers(itemType);
			var memberBindings = new List<MemberSerializationBinding>(members.Length);
			foreach (var member in members) {
				var propertyType = member.PropertyType;
				
				// Ensure we have a serializer for the member type
				if (propertyType != typeof(object) && !factory.HasSerializer(propertyType))
					AssembleRecursively(factory, propertyType);

				// We don't use the member type serializer but instead use a FactorySerializer to ensure cyclic/polymorphic references are handled correctly
				var memberSerializer = (IItemSerializer) typeof(FactorySerializer<>).MakeGenericType(propertyType).ActivateWithCompatibleArgs(factory);
				memberBindings.Add(new(member, memberSerializer.AsReferenceSerializer()));
			}
			
			// Configure the composite serializer instance (which is already registered)
			var itemTypeLocal = itemType;
			compositeSerializer
				.GetType()
				.GetMethod(nameof(CompositeSerializer<object>.Configure), BindingFlags.Instance | BindingFlags.NonPublic)
				.Invoke(compositeSerializer, [() => itemTypeLocal.ActivateWithCompatibleArgs(), memberBindings.ToArray()]);
			
			return serializer;
		}

		IEnumerable<Type> GetUnregisteredComponentTypes(SerializerFactory factory, Type type, HashSet<Type> alreadyVisited = null) {
			alreadyVisited ??= new HashSet<Type>();

			// List<Type>
			// Type[]
			// Type1<Type2, Type3>

			// Avoid recursive loops
			if (alreadyVisited.Contains(type))
				yield break; 
			alreadyVisited.Add(type);
			
			// Case 1: There is an explicit serializer for this type, no component types need to be assembled
			if (factory.HasSerializer(type))
				yield break; 
			

			// Case 2: Array element type may need assembling
			if (type.IsArray) {
				var elementType = type.GetElementType();
				if (!factory.HasSerializer(elementType)) {
					foreach(var elementTypeUnregisteredComponentTypes in GetUnregisteredComponentTypes(factory, elementType, alreadyVisited))
						yield return elementTypeUnregisteredComponentTypes;
					yield return elementType;
				}
			}

			// Case 4: Serializer for generic type definition exists but not for generic type arguments 
			// e.g. List<UnregType>, Dictionary<UnregType1, UnregType2>, etc
			if (type.IsConstructedGenericType && factory.HasSerializer(type.GetGenericTypeDefinition())) {
				foreach (var genericArgumentType in type.GetGenericArguments().Where(x => !factory.HasSerializer(x))) {
					foreach(var subType in GetUnregisteredComponentTypes(factory, genericArgumentType, alreadyVisited))
						yield return subType;
					yield return genericArgumentType;
				}
			}
		}
	}


	#endregion

	#region Inner Types

	public class Registration {
		public long TypeCode { get; set; }
		public Type DataType { get; set; }
		public Type SerializerType { get; set; }
		public Func<Registration, Type, IItemSerializer> Factory { get; set; }
		public SerializerFactory SerializerFactory { get; set; }
	}

	#endregion
}
