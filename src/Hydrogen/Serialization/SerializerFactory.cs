// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Hydrogen.FastReflection;

namespace Hydrogen;

public class SerializerFactory {
	internal const int RegistrationCodeStart = 10;  // 0 - 9 reserved for special types (i.e. 0 is used to indicate cyclic reference)

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
		_getSerializerCache = new ActionCache<Type, IItemSerializer>(GetSerializerInternal,  keyComparer: TypeEquivalenceComparer.Instance);
		_readOnly = false;
		_nextFactory = null;
	}

	public SerializerFactory(SerializerFactory baseFactory) : this() {
		CopyRegistrations(baseFactory);
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

	public IEnumerable<Type> RegisteredTypes => _registrationsByType.Keys;

	public static SerializerFactory Default { get; }

	public static void RegisterDefaults(SerializerFactory factory) {
		// register self-assembling factory for object
		
		// fundamental serialization types
		factory.Register(typeof(Array), typeof(ArraySerializer<>));  // special general array serializer
		factory.Register(typeof (Nullable<>), typeof (NullableStructSerializer<>));
		

		// primitive & base .net types
		factory.Register(PrimitiveSerializer<bool>.Instance);
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
		factory.Register(NullableStructSerializer<bool>.Instance);
		factory.Register(NullableStructSerializer<byte>.Instance);
		factory.Register(NullableStructSerializer<char>.Instance);
		factory.Register(NullableStructSerializer<ushort>.Instance);
		factory.Register(NullableStructSerializer<short>.Instance);
		factory.Register(NullableStructSerializer<uint>.Instance);
		factory.Register(NullableStructSerializer<int>.Instance);
		factory.Register(NullableStructSerializer<ulong>.Instance);
		factory.Register(NullableStructSerializer<long>.Instance);
		factory.Register(NullableStructSerializer<float>.Instance);
		factory.Register(NullableStructSerializer<double>.Instance);
		factory.Register(NullableStructSerializer<decimal>.Instance);
		factory.Register(new NullableStructSerializer<DateTime>(DateTimeSerializer.Instance));
		factory.Register(new NullableStructSerializer<TimeSpan>(TimeSpanSerializer.Instance));
		factory.Register(new NullableStructSerializer<DateTimeOffset>(DateTimeOffsetSerializer.Instance));
		factory.Register(new NullableStructSerializer<Guid>(GuidSerializer.Instance));

		// other base .net types
		factory.Register(new StringSerializer().AsNullable());
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>));

		// general collections
		factory.Register(ByteArraySerializer.Instance);
		factory.Register(typeof(IEnumerable<>), typeof(EnumerableSerializer<>));
		factory.Register(typeof(ICollection<>), typeof(CollectionInterfaceSerializer<>));
		factory.Register(typeof(Collection<>), typeof(CollectionSerializer<>));
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>));
		factory.Register(typeof(List<>), typeof(ListSerializer<>));
		factory.Register(typeof(IDictionary<,>), typeof(DictionaryInterfaceSerializer<,>));
		factory.Register(typeof(Dictionary<,>), typeof(DictionarySerializer<,>));

	}

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

	#region Register


	public void RegisterEnum<TEnum>() where TEnum : struct, Enum 
		=> RegisterEnum(typeof(TEnum));

	public void RegisterEnum(Type enumType) {
		Guard.ArgumentNotNull(enumType, nameof(enumType));
		Guard.Argument(enumType.IsEnum, nameof(enumType), $"Type {enumType.Name} is not an enum");
		var enumSerializer = (IItemSerializer) typeof(EnumSerializer<>).MakeGenericType(enumType).ActivateWithCompatibleArgs();
		var nullableEnumSerializer = (IItemSerializer) typeof(NullableStructSerializer<>).MakeGenericType(enumType).ActivateWithCompatibleArgs(enumSerializer);
		Register(GenerateTypeCode(), enumType, enumSerializer.GetType(), enumSerializer, null);
		Register(GenerateTypeCode(), typeof(Nullable<>).MakeGenericType(enumType), nullableEnumSerializer.GetType(), nullableEnumSerializer, null);
	}

	public void Register<TItem>(IItemSerializer<TItem> serializerInstance)
		=> Register(GenerateTypeCode(), typeof(TItem), serializerInstance.GetType(), serializerInstance, null);

	public void Register<TItem, TSerializer>() where TSerializer : IItemSerializer<TItem>, new()
		=> Register(GenerateTypeCode(), typeof(TItem), typeof(TSerializer), null, (_,_) => new TSerializer());

	public void Register<TItem>(Type serializerType) {
		Register(GenerateTypeCode(), typeof(TItem), serializerType, null, null);
	}

	public void Register(Type dataType, Type serializerType) {
		Register(GenerateTypeCode(), dataType, serializerType, null, null);
	}

	public void Register(long typeCode, Type dataType, Type serializerType, IItemSerializer serializerInstance, Func<Registration, Type, IItemSerializer> factory) {
		Guard.ArgumentNotNull(dataType, nameof(dataType));
		Guard.ArgumentNotNull(serializerType, nameof(serializerType));
		Guard.Against(dataType == typeof(object), $"Cannot register serializer for {nameof(Object)} type");
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
			registration.Factory =  (_, _) => serializerInstance;
		} else if (factory is null) {
			// get all sub-types
			registration.Factory = (serializerFactory, requested) => CreateSerializerInstance(serializerFactory, requested, dataType, serializerType);
		}

		_registrations.Add(typeCode, registration);
		_registrationsByType.Add(dataType, typeCode);

		_getSerializerCache.Flush();
		_fromSerializerHierarchyCache.Flush();
	}

	public void RegisterAutoBuild<T>() 
		=> RegisterAutoBuild(typeof(T));

	public void RegisterAutoBuild(Type dataType) 
		=> AssembleInternal(dataType, true);

	#endregion

	#region Has Serializer

	public bool HasSerializer(Type dataType) => TryFindCompatibleSerializer(dataType, out _, out _);

	#endregion

	#region Get Serializer

	public IItemSerializer<TType> GetSerializer<TType>() 
		=> GetSerializer<TType>(typeof(TType));

	public IItemSerializer<TBase> GetSerializer<TBase, TConcrete>() where TConcrete : TBase 
		=> GetSerializer<TBase>(typeof(TConcrete));
	
	public IItemSerializer<TSerializerDataType> GetSerializer<TSerializerDataType>(Type dataType) {
		Guard.ArgumentNotNull(dataType, nameof(dataType));
		var serializerDataType = typeof(TSerializerDataType);
		Guard.Argument(dataType.IsAssignableTo(serializerDataType), nameof(dataType), $"{dataType.ToStringCS()} must be assignable to {serializerDataType.ToStringCS()}");
		var serializerObj = GetSerializer(dataType);

		var serializer = serializerObj as IItemSerializer<TSerializerDataType>;
		if (serializer is null) {
			serializer = new CastedSerializer<TSerializerDataType>(serializerObj);
		}

		return serializer;
	}

	public IItemSerializer GetSerializer(Type dataType) => _getSerializerCache[dataType];

	// Called by _getSerializerCache to get serializer
	private IItemSerializer GetSerializerInternal(Type dataType) {
		Guard.ArgumentNotNull(dataType, nameof(dataType));
  	    var registration = FindCompatibleSerializer(dataType, out _);
		return registration.Factory(registration, dataType);
	}

	#endregion

	#region Assemble

	public IItemSerializer<T> Assemble<T>() 
		=> (IItemSerializer<T>)AssembleInternal(typeof(T), false);

	public IItemSerializer Assemble(Type type) 
		=> AssembleInternal(type, false);

	private IItemSerializer AssembleInternal(Type itemType, bool registerTypes) {
		
		// During the construction, a factory is required to store generated serializers.
		var factory = registerTypes ? this : new SerializerFactory(this);

		var assembledSerializer = AssembleRecursively(factory, itemType);

		return assembledSerializer;

		// TODO: support nested-types by intelligently tracking parent 
		IItemSerializer AssembleRecursively(SerializerFactory factory, Type itemType) {
			//Guard.Ensure(!alreadyProcessed.ContainsKey(itemType), $"A cyclic-type dependency was detected while building a serializer for type {itemType.ToStringCS()}");

			// If serializer already exists for this type in factory, use that
			if (factory.HasSerializer(itemType))
				return factory.GetSerializer(itemType);

			// Special Case: if we're serializing an enum (or nullable enum), we register it with the factory now and return
			if (itemType.IsEnum || itemType.IsConstructedGenericTypeOf(typeof(Nullable<>)) && itemType.GenericTypeArguments[0].IsEnum) {
				factory.RegisterEnum(itemType.IsEnum ? itemType : itemType.GenericTypeArguments[0]);
				return factory.GetSerializer(itemType);
			}

			// No serializer registered so we need to assemble one. First, we need to register the
			// serializer (before it is assembled) as it may recursively refer to itself. So we activate
			// a CompositeSerializer with no members (we'll configure it later)
			var serializer = 
				(IItemSerializer) typeof(CompositeSerializer<>)
				.MakeGenericType(itemType)
				.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)
				.Invoke(null);

			// register serializer instance now as it may be re-used in component serializers (recursive types)
			if (itemType != typeof(object))
				factory.Register(factory.GenerateTypeCode(), itemType, serializer.GetType(), serializer, null);

			// Create the member serializers
			var members = SerializerBuilder.GetSerializableMembers(itemType);
			var memberBindings = new List<MemberSerializationBinding>(members.Length);
			foreach (var member in members) {
				var propertyType =  member.PropertyType;
				
				// Ensure we have a serializer for the member type
				if (propertyType != typeof(object)  && !factory.HasSerializer(propertyType))
					AssembleRecursively(factory, propertyType);

				// We don't use the member type serializer but instead use a FactorySerializer to ensure cyclic/polymorphic references are handled correctly
				var memberSerializer = (IItemSerializer) typeof(CyclicReferenceAwareSerializer<>).MakeGenericType(propertyType).ActivateWithCompatibleArgs(factory);
				memberBindings.Add(new(member, memberSerializer.AsSanitized()));
			}
			
			// Configure the serializer instance (registered already)
			var itemTypeLocal = itemType;
			serializer
				.GetType()
				.GetMethod(nameof(CompositeSerializer<object>.ConfigureInternal), BindingFlags.Instance | BindingFlags.NonPublic)
				.FastInvoke(serializer, () => itemTypeLocal.ActivateWithCompatibleArgs(), memberBindings.ToArray());
			
			return serializer;
		}

	}

	#endregion


	#region Aux
	
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

	private long GenerateTypeCode() => _registrations.Count > 0 ? _registrations.Keys.Max() + 1 : RegistrationCodeStart;

	private static IItemSerializer CreateSerializerInstance(Registration registration, Type requestedDataType, Type registeredDataType, Type registeredSerializerType) {
		Guard.Argument(!requestedDataType.IsGenericTypeDefinition, nameof(requestedDataType), $"Requested data type {requestedDataType.Name} cannot be a generic type definition");
		if (registeredDataType.IsGenericTypeDefinition)
			Guard.Ensure(requestedDataType.IsConstructedGenericTypeOf(registeredDataType), $"Constructed type {requestedDataType.Name} is not a constructed generic type of {registeredDataType.Name}");
		//var subTypes = requestedDataType.IsArray ? new [] { requestedDataType.GetElementType() } : requestedDataType.GetGenericArguments();
		var subTypes = requestedDataType switch {
			{ IsArray: true } => new [] { requestedDataType.GetElementType() },
			{ IsConstructedGenericType: true } => requestedDataType.GetGenericArguments(),
		};

		var subTypeSerializers = requestedDataType switch {
			_ => subTypes.Select(registration.SerializerFactory.GetSerializer).ToArray()
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
			//_ when dataType == typeof(Enum) => subTypes[0],
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

	#endregion

	public class Registration {
		public long TypeCode { get; set; }
		public Type DataType { get; set; }
		public Type SerializerType { get; set; }
		public Func<Registration, Type, IItemSerializer> Factory { get; set; }

		public SerializerFactory SerializerFactory { get; set; }
	}
}
