// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hydrogen;

public class SerializerFactory {
	internal const int PermanentTypeCodeStartDefault = 10; 
	internal const int EphemeralTypeCodeStartDefault = 65536; // Type codes for ephemerally registered types start here. This allows you to update your core serializer registrations over time without affecting previous serialized objects.

	private readonly IDictionary<long, Registration> _registrations;
	private readonly BijectiveDictionary<Type, long> _registrationsByType;
	private readonly ICache<Type, RecursiveDataType<long>> _getSerializerHierarchyCache;
	private readonly ICache<RecursiveDataType<long>, IItemSerializer> _fromSerializerHierarchyCache;
	private readonly IDictionary<Type, IItemSerializer> _activatedSerializers;
	private bool _readOnly;

	static SerializerFactory() {
		Default = new SerializerFactory();
		RegisterDefaults(Default);
		Default.FinishRegistrations();
		
	}

	public SerializerFactory() {
		_registrations = new Dictionary<long, Registration>();
		_registrationsByType = new BijectiveDictionary<Type, long>(new TypeEquivalenceComparer(), EqualityComparer<long>.Default);
		_getSerializerHierarchyCache = new ActionCache<Type, RecursiveDataType<long>>(GetSerializerHierarchyInternal, keyComparer: TypeEquivalenceComparer.Instance);
		_fromSerializerHierarchyCache = new ActionCache<RecursiveDataType<long>, IItemSerializer>(FromSerializerHierarchyInternal);
		_activatedSerializers = new Dictionary<Type, IItemSerializer>(TypeEquivalenceComparer.Instance);
		//_getSerializerCache = new ActionCache<Type, IItemSerializer>(GetCachedSerializer_Fetch,  keyComparer: TypeEquivalenceComparer.Instance);
		_readOnly = false;
		}

	public SerializerFactory(SerializerFactory baseFactory) : this() {
		CopyRegistrations(baseFactory);
	}

	public IEnumerable<Type> RegisteredTypes => _registrationsByType.Keys;

	public static SerializerFactory Default { get; }

	internal long MinGeneratedTypeCode { get; set; } = PermanentTypeCodeStartDefault;

	internal long MaxGeneratedTypeCode { get; private set; } = 0;

	#region Registration

	public static void RegisterDefaults(SerializerFactory factory) {
		// register self-assembling factory for object
		
		// fundamental serialization types
		factory.Register<object, PureObjectSerializer>();
		factory.Register(typeof (Nullable<>), typeof (NullableSerializer<>)); // nullable struct serializer
		factory.Register(typeof(Tuple<>) , typeof(TupleSerializer<>)); 
		factory.Register(typeof(Tuple<,>) , typeof(TupleSerializer<,>));
		factory.Register(typeof(Tuple<,,>) , typeof(TupleSerializer<,,>));
		factory.Register(typeof(Tuple<,,,>) , typeof(TupleSerializer<,,,>));
		factory.Register(typeof(Tuple<,,,,>) , typeof(TupleSerializer<,,,,>));
		factory.Register(typeof(Tuple<,,,,,>) , typeof(TupleSerializer<,,,,,>));
		factory.Register(typeof(Tuple<,,,,,,>) , typeof(TupleSerializer<,,,,,,>));
		factory.Register(typeof(Tuple<,,,,,,,>) , typeof(TupleSerializer<,,,,,,,>));
		factory.Register(typeof(ValueTuple<>) , typeof(ValueTupleSerializer<>)); 
		factory.Register(typeof(ValueTuple<,>) , typeof(ValueTupleSerializer<,>));
		factory.Register(typeof(ValueTuple<,,>) , typeof(ValueTupleSerializer<,,>));
		factory.Register(typeof(ValueTuple<,,,>) , typeof(ValueTupleSerializer<,,,>));
		factory.Register(typeof(ValueTuple<,,,,>) , typeof(ValueTupleSerializer<,,,,>));
		factory.Register(typeof(ValueTuple<,,,,,>) , typeof(ValueTupleSerializer<,,,,,>));
		factory.Register(typeof(ValueTuple<,,,,,,>) , typeof(ValueTupleSerializer<,,,,,,>));
		//factory.Register(typeof(ValueTuple<,,,,,,,>), typeof(ValueTupleSerializer<,,,,,,,>));

		// base types (primitives, common use, etc)
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
		factory.Register(CVarIntSerializer.Instance);
		factory.Register(VarIntSerializer.Instance);
		factory.Register(DateTimeSerializer.Instance);
		factory.Register(TimeSpanSerializer.Instance);
		factory.Register(DateTimeOffsetSerializer.Instance);
		factory.Register(GuidSerializer.Instance);
		
		// nullable base types 
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
		factory.Register(NullableSerializer<CVarInt>.Instance);
		factory.Register(NullableSerializer<VarInt>.Instance);
		factory.Register(NullableSerializer<DateTime>.Instance);
		factory.Register(NullableSerializer<TimeSpan>.Instance);
		factory.Register(NullableSerializer<DateTimeOffset>.Instance);
		factory.Register(NullableSerializer<Guid>.Instance);

		// other base .net types
		factory.Register(new StringSerializer());
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>));
		factory.Register(TypeSerializer.Instance);
		factory.Register(AssemblySerializer.Instance);
		
		// general collections
		factory.Register(typeof(Array), typeof(ArraySerializer<>));  // special general array serializer
		factory.Register(ByteArraySerializer.Instance);
		factory.Register<ArrayList, ArrayListSerializer>(factoryAtActivation => new ArrayListSerializer(factoryAtActivation, SizeDescriptorStrategy.UseCVarInt));
		factory.Register(typeof(Collection<>), typeof(CollectionSerializer<>));
		factory.Register(typeof(List<>), typeof(ListSerializer<>));
		factory.Register(typeof(ExtendedList<>), typeof(ExtendedListSerializer<>));
		factory.Register(typeof(Dictionary<,>), typeof(DictionarySerializer<,>));
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
				Owner = this  // when copying another factory, we set the new factory as the owner of the registration
			};
			_registrations.Add(registration.Key, newRegistration);
			_registrationsByType.Add(registration.Value.DataType, registration.Key);
			MaxGeneratedTypeCode = Math.Max(MaxGeneratedTypeCode, registration.Value.TypeCode);
		}
	}

	public SerializerFactory SetMinTypeCode(long min) {
		MinGeneratedTypeCode = min;
		return this;
	}

	public SerializerFactory RegisterEnum<TEnum>(long? typeCode = null) where TEnum : struct, Enum 
		=> RegisterEnum(typeof(TEnum), typeCode);

	public SerializerFactory RegisterEnum(Type enumType, long? typeCode = null) {
		Guard.ArgumentNotNull(enumType, nameof(enumType));
		Guard.Argument(enumType.IsEnum, nameof(enumType), $"Type {enumType.Name} is not an enum");
		var enumSerializer = (IItemSerializer) typeof(EnumSerializer<>).MakeGenericType(enumType).ActivateWithCompatibleArgs();
		var nullableEnumSerializer = (IItemSerializer) typeof(NullableSerializer<>).MakeGenericType(enumType).ActivateWithCompatibleArgs(enumSerializer);
		var code = typeCode ?? GenerateTypeCode();
		if (!ContainsSerializer(enumType))
			RegisterInternal(code, enumType, enumSerializer.GetType(), enumSerializer, null);

		var nullableEnumType  = typeof(Nullable<>).MakeGenericType(enumType);
		if (!ContainsSerializer(nullableEnumType))
			RegisterInternal(code + 1, nullableEnumType, nullableEnumSerializer.GetType(), nullableEnumSerializer, null);
		return this;
	}

	public SerializerFactory Register<TItem>(IItemSerializer<TItem> serializerInstance, long? typeCode = null) {
		 RegisterInternal(typeCode ?? GenerateTypeCode(), typeof(TItem), serializerInstance.GetType(), serializerInstance, null);
		return this;
	}

	public SerializerFactory Register<TItem, TSerializer>(long? typeCode = null) where TSerializer : IItemSerializer<TItem>, new() {
		RegisterInternal(typeCode ?? GenerateTypeCode(), typeof(TItem), typeof(TSerializer), null, (_,_) => new TSerializer());
		return this;
	}

	public SerializerFactory Register<TItem, TSerializer>(Func<TSerializer> factory, long? typeCode = null) 
		=> Register<TItem, TSerializer>(_ => factory(), typeCode);

	public SerializerFactory Register<TItem, TSerializer>(Func<SerializerFactory, TSerializer> factory, long? typeCode = null) {
		RegisterInternal(typeCode ?? GenerateTypeCode(), typeof(TItem), typeof(TSerializer), null, (r,_) => (IItemSerializer)factory(r.Owner));
		return this;
	}

	public SerializerFactory Register<TItem>(Type serializerType, long? typeCode = null) {
		RegisterInternal(typeCode ?? GenerateTypeCode(), typeof(TItem), serializerType, null, null);
		return this;
	}

	public SerializerFactory Register(Type dataType, Type serializerType, long? typeCode = null) {
		RegisterInternal(typeCode ?? GenerateTypeCode(), dataType, serializerType, null, null);
		return this;
	}

	public SerializerFactory Register(Type dataType, IItemSerializer serializerInstance, long? typeCode = null) {
		RegisterInternal(typeCode ?? GenerateTypeCode(), dataType, serializerInstance.GetType(), serializerInstance, null);
		return this;
	}

	public SerializerFactory RegisterAutoBuild<T>(long? typeCode = null) 
		=> RegisterAutoBuild(typeof(T), typeCode);

	public SerializerFactory RegisterAutoBuild(Type dataType, long? typeCode = null) {
		Guard.ArgumentNotNull(dataType, nameof(dataType));
		Guard.Argument(!dataType.ContainsGenericParameters, nameof(dataType), "Cannot auto-build serializers for open generic types");
		SerializerBuilder.FactoryAssemble(this, dataType, true, typeCodeStart: typeCode);
		return this;
	}
	
	internal void RegisterInternal(long typeCode, Type dataType, Type serializerType, IItemSerializer serializerInstance, Func<Registration, Type, IItemSerializer> factory) {
		Guard.ArgumentNotNull(dataType, nameof(dataType));
		Guard.ArgumentNotNull(serializerType, nameof(serializerType));
		Guard.Argument(!serializerType.IsConstructedGenericTypeOf(typeof(ReferenceSerializer<>)), nameof(serializerType), "Reference serializers cannot be registered directly");
		if (!dataType.IsAbstract)
			Guard.Argument(!serializerType.IsConstructedGenericTypeOf(typeof(PolymorphicSerializer<>)), nameof(serializerType), $"A polymorphic serializer cannot be registered directly for non-abstract type {dataType.ToStringCS()}");
		
		//Guard.Against(dataType == typeof(object), $"Cannot register serializer for {nameof(Object)} type");
		Guard.Ensure(!_readOnly, "Factory is read-only and cannot be changed");

		if (dataType == typeof(Array)) {
			// Special registration for array serialization
			Guard.Ensure(serializerType.IsPartialOrGenericTypeDefinition(), $"Serializer type {serializerType.Name} must be a generic type definition for data type {dataType.ToStringCS()}");
			Guard.Argument(serializerType.IsSubtypeOfGenericType(typeof(IItemSerializer<>), out var matchedGenericType) &&  matchedGenericType.ContainsGenericParameters && matchedGenericType.GenericTypeArguments[0].IsArray, nameof(serializerType), $"Serializer type must implement IItemSerializer<T[]>");
		} else if (dataType.IsFullyConstructed() || serializerType.IsActivatable()) {
			Guard.Ensure(dataType.IsFullyConstructed(), $"Data type {dataType.Name} must be a fully constructed type for serializer type {serializerType.ToStringCS()}");
			Guard.Ensure(serializerType.IsActivatable(), $"Serializer type {serializerType.Name} must be an instantiable type for serializer type {dataType.ToStringCS()}");
			Guard.Argument(serializerType.IsSubTypeOf(typeof(IItemSerializer<>).MakeGenericType(dataType)), nameof(serializerType), $"Serializer type must implement IItemSerializer<{dataType.ToStringCS()}>");		
		} else {
			Guard.Ensure(dataType.IsPartialOrGenericTypeDefinition(), $"Data type {dataType.Name} must be a generic type definition for serializer type {serializerType.Name}");
			Guard.Ensure(serializerType.IsPartialOrGenericTypeDefinition(), $"Serializer type {serializerType.Name} must be a generic type definition for data type {dataType.Name}");
			Guard.Argument(serializerType.IsSubtypeOfGenericType(typeof(IItemSerializer<>).MakeGenericType(dataType)), nameof(serializerType), $"Serializer type must implement IItemSerializer<{dataType.ToStringCS()}>");
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

		// Track max type code
		MaxGeneratedTypeCode = Math.Max(MaxGeneratedTypeCode, typeCode);

		// add registration (we fix up object factory after, since it requires a reference to the registration)
		var registration = new Registration {
			TypeCode = typeCode,
			DataType = dataType,
			SerializerType = serializerType,
			Factory = null,
			Owner = this
		};

		// Ensure instance is of correct type
		if (serializerInstance != null) {
			if (!serializerType.IsInstanceOfType(serializerInstance))
				throw new InvalidOperationException($"Serializer instance is not of type {serializerType.Name}");

			Guard.Ensure(factory is null, "Factory must be null if serializer instance is provided");

			factory =  (_, _) => serializerInstance;
		} else if (factory is null) {
			// get all sub-types
			factory = (serializerFactory, requested) => CreateSerializerInstance(serializerFactory, requested, dataType, serializerType);
		} 

		registration.Factory = factory;
		_registrations.Add(typeCode, registration);
		_registrationsByType.Add(dataType, typeCode);

		_activatedSerializers.Clear();
		_fromSerializerHierarchyCache.Purge();
	}

	internal long GenerateTypeCode() => _registrations.Count == 0 ? MinGeneratedTypeCode : Math.Max(MaxGeneratedTypeCode + 1, MinGeneratedTypeCode);

	private static IItemSerializer CreateSerializerInstance(Registration registration, Type requestedDataType, Type registeredDataType, Type registeredSerializerType) {
		Guard.Argument(!requestedDataType.IsGenericTypeDefinition, nameof(requestedDataType), $"Requested data type {requestedDataType.Name} cannot be a generic type definition");
		if (registeredDataType.IsGenericTypeDefinition)
			Guard.Ensure(requestedDataType.IsConstructedGenericTypeOf(registeredDataType), $"Constructed type {requestedDataType.Name} is not a constructed generic type of {registeredDataType.Name}");

		var subTypes = requestedDataType switch {
			{ IsArray: true } => new [] { requestedDataType.GetElementType() },
			{ IsConstructedGenericType: true } => requestedDataType.GetGenericArguments(),
		};

		var subTypeSerializers = requestedDataType switch {
			_ => subTypes.Select(type => registration.Owner.GetSerializer(type)).ToArray()
		};
		var serializerType = registeredSerializerType;
		if (serializerType.IsGenericTypeDefinition)
			serializerType = serializerType.MakeGenericType(subTypes);
		return (IItemSerializer) TypeActivator.ActivateWithCompatibleArgs(serializerType, subTypeSerializers);
	}

	#endregion

	#region Serializer Accessing

	public bool ContainsSerializer<T>() => ContainsSerializer(typeof(T));

	public bool ContainsSerializer(Type dataType) => ContainsSerializer(dataType, out _);

	public bool ContainsSerializer(Type dataType, out HashSet<Type> missingSerializers) 
		=> TryFindRegistration(dataType, out _, out missingSerializers);

	public IItemSerializer<T> GetPureSerializer<T>() 
		=> (IItemSerializer<T>)GetPureSerializer(typeof(T));

	public IItemSerializer GetPureSerializer(Type dataType) {
		if (!TryGetPureSerializer(dataType, out var serializer, out var missingSerializers))
			throw new InvalidOperationException($"No serializer was available for type {dataType.ToStringCS()} (missing serializers for {missingSerializers.Select(x => x.ToStringCS()).ToDelimittedString(", ")})");
		return serializer;
	}

	public bool TryGetPureSerializer<T>(out IItemSerializer<T> serializer, out HashSet<Type> missingSerializers) {
		if (TryGetPureSerializer(typeof(T), out var serializerObj, out missingSerializers)) {
			serializer = null;
			return false;
		}
		serializer = (IItemSerializer<T>)serializerObj;
		return true;
	}

	public bool TryGetPureSerializer(Type dataType, out IItemSerializer serializer, out HashSet<Type> missingSerializers) {
		Guard.ArgumentNotNull(dataType, nameof(dataType));
		if (_activatedSerializers.TryGetValue(dataType, out serializer)) {
			missingSerializers = null;
			return true;
		}

		if (!TryFindRegistration(dataType, out var registration, out missingSerializers))
			return false;

		serializer = registration.Factory(registration, dataType);
		_activatedSerializers.Add(dataType, serializer);
		return true;
	}

	public IItemSerializer<T> GetSerializer<T>(ReferenceSerializerMode mode = ReferenceSerializerMode.Default) 
		=> GetPureSerializer<T>().AsWrapped(this, mode);

	public IItemSerializer GetSerializer(Type type, ReferenceSerializerMode mode = ReferenceSerializerMode.Default) 
		=> GetPureSerializer(type).AsWrapped(this, mode);

	public bool TryGetSerializer<T>(ReferenceSerializerMode mode, out IItemSerializer<T> serializer, out HashSet<Type> missingSerializers) {
		if (!TryGetSerializer(typeof(T), mode, out var serializerObj, out missingSerializers)) {
			serializer = null;
			return false;
		}
		serializer = (IItemSerializer<T>)serializerObj;
		return true;
	}

	public bool TryGetSerializer(Type dataType, ReferenceSerializerMode mode, out IItemSerializer serializer, out HashSet<Type> missingSerializers) {
		if (!TryGetPureSerializer(dataType, out serializer, out missingSerializers)) {
			return false;
		}
		serializer = serializer.AsWrapped(this, mode);
		return true;
	}

	#endregion
	
	#region Serializer Hierarchy 

	public RecursiveDataType<long> GetSerializerHierarchy(Type dataType) 
		=> _getSerializerHierarchyCache[dataType];

	private RecursiveDataType<long> GetSerializerHierarchyInternal(Type type) {
		Guard.ArgumentNotNull(type, nameof(type));
		Guard.Argument(!type.IsGenericTypeDefinition, nameof(type), $"Type {type.ToStringCS()} must not be a generic type definition");
		RecursiveDataType<long>[] childSerializers;
	
		var serializerRegistration = FindRegistration(type);
		if (serializerRegistration.DataType == typeof(Array)) {
			// arrays are special case, we matched generic array serializer, so sub-serializers are for element type
			childSerializers = [GetSerializerHierarchy(type.GetElementType())];
		} else if (serializerRegistration.DataType == typeof(Enum)) { 
			// enums are special case, we matched enum serializer, so sub-serializers are for enum type
			childSerializers = [GetSerializerHierarchy(type.GetEnumUnderlyingType())];
		} else {
			// case (2), now get any sub-serializers required for generic serializer
			childSerializers =
				serializerRegistration.SerializerType.IsGenericTypeDefinition ?
					type.GetGenericArguments().Select(GetSerializerHierarchy).ToArray() :
					Enumerable.Empty<RecursiveDataType<long>>().ToArray();
		}

		return new RecursiveDataType<long>(serializerRegistration.TypeCode, childSerializers);
	}

	public IItemSerializer FromSerializerHierarchy(RecursiveDataType<long> serializerHierarchy) 
		=> _fromSerializerHierarchyCache[serializerHierarchy];

	private IItemSerializer FromSerializerHierarchyInternal(RecursiveDataType<long> serializerHierarchy) {
		var registration = GetRegistration(serializerHierarchy.State);
		var type = CalculateTypeFromHierarchy(serializerHierarchy);
		return registration.Factory(registration, type);
	}

	public long CountSubSerializers(long typeCode) {
		if (!_registrations.TryGetValue(typeCode, out var registration))
			throw new InvalidOperationException($"No serializer registered for type code {typeCode}");
		return registration.SerializerType.IsGenericTypeDefinition ? registration.SerializerType.GetGenericArguments().Length : 0;
	}

	private Type CalculateTypeFromHierarchy(RecursiveDataType<long> serializerHierarchy) {
		var registration = GetRegistration(serializerHierarchy.State);
		var dataType = registration.DataType;
		if (!dataType.IsGenericTypeDefinition && dataType != typeof(Array))
			return dataType;
		var subTypes = serializerHierarchy.SubStates.Select(CalculateTypeFromHierarchy).ToArray();
		
		return dataType switch {
			_ when dataType == typeof(Array) => subTypes[0].MakeArrayType(),
			_ => dataType.MakeGenericType(subTypes)
		};
	}

	#endregion

	#region Registrations
	
	public Registration GetRegistration(long typeCode) {
		if (!TryGetRegistration(typeCode, out var registration))
			throw new InvalidOperationException($"No serializer registered for type code {typeCode}");
		return registration;
	}

	public bool TryGetRegistration(long typeCode, out Registration registration) 
		=> _registrations.TryGetValue(typeCode, out registration);

	public Registration FindRegistration(Type dataType) {
		if (!TryFindRegistration(dataType, out var registration, out _))
			throw new InvalidOperationException($"No serializer registered for type {dataType.Name}");
		return registration;
	}

	public bool TryFindRegistration(Type dataType, out Registration registration, out HashSet<Type> missingSerializers) {
		var foundSerializers = new Dictionary<Type, Registration>();
		missingSerializers = [];
		CalculateAllRegistrationsRequired(dataType, foundSerializers, missingSerializers);
		if (missingSerializers.Count == 0) {
			registration = foundSerializers[dataType];
			return true;
		}
		registration = null;
		return false;
	}

	public void CalculateAllRegistrationsRequired(Type dataType, IDictionary<Type, Registration> foundRegistrations, HashSet<Type> missingSerializers) {

		if (foundRegistrations.ContainsKey(dataType))
			return;
		
		if (_registrationsByType.TryGetValue(dataType, out var typeCode)) {
			// Try direct lookup
			var registration = _registrations[typeCode];
			foundRegistrations.Add(dataType, registration);
		} else if (dataType.IsArray && _registrationsByType.TryGetValue(typeof(Array), out typeCode)) {
			// Special case for arrays
			var registration = _registrations[typeCode];
			foundRegistrations.Add(dataType, registration);
			CalculateAllRegistrationsRequired(dataType.GetElementType(), foundRegistrations, missingSerializers);
		} else if (dataType.IsGenericType && _registrationsByType.TryGetValue(dataType.GetGenericTypeDefinition(), out typeCode)) {
			// We have the serializer for generic type definition
			var registration = _registrations[typeCode];
			foundRegistrations.Add(dataType, registration);

			// need to check have serializers for generic arguments (if applicable)
			if (dataType.IsConstructedGenericType) 
				foreach(var genericArgType in dataType.GetGenericArguments())
					CalculateAllRegistrationsRequired(genericArgType, foundRegistrations, missingSerializers);
		} else {
			missingSerializers.Add(dataType);
		}
	}

	public IEnumerable<Registration> GetRegistrationsAfterTypeCode(long minCodeExclusive) => _registrations.Where(x => x.Key > minCodeExclusive).Select(x => x.Value);

	public void FinishRegistrations() {
		if (_readOnly)
			throw new InvalidOperationException("Factory is already read-only");
		_readOnly = true;
	}


	#endregion

	#region Inner Types

	public record Registration {
		public long TypeCode { get; set; }
		public Type DataType { get; set; }
		public Type SerializerType { get; set; }
		public Func<Registration, Type, IItemSerializer> Factory { get; set; }
		public SerializerFactory Owner { get; set; }
	}

	#endregion
}
