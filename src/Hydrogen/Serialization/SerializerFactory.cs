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
using System.Linq;
using Hydrogen.FastReflection;

namespace Hydrogen;

public class SerializerFactory {
	private readonly IDictionary<long, Registration> _registrations;
	private readonly BijectiveDictionary<Type, long> _registrationsByType;
	private readonly ICache<Type, RecursiveDataType<long>> _getSerializerHierarchyCache;
	private readonly ICache<RecursiveDataType<long>, IItemSerializer> _fromSerializerHierarchyCache;
	private readonly ICache<Type, IItemSerializer> _getSerializerCache;

	static SerializerFactory() {
		Default = new SerializerFactory();
		RegisterDefaults(Default);
	}

	public SerializerFactory() {
		_registrations = new Dictionary<long, Registration>();
		_registrationsByType = new BijectiveDictionary<Type, long>(new TypeEquivalenceComparer(), EqualityComparer<long>.Default);
		_getSerializerHierarchyCache = new ActionCache<Type, RecursiveDataType<long>>(GetSerializerHierarchyInternal, keyComparer: TypeEquivalenceComparer.Instance);
		_fromSerializerHierarchyCache = new ActionCache<RecursiveDataType<long>, IItemSerializer>(FromSerializerHierarchyInternal);
		_getSerializerCache = new ActionCache<Type, IItemSerializer>(GetSerializerInternal,  keyComparer: TypeEquivalenceComparer.Instance);
	}

	public IEnumerable<Type> RegisteredTypes => _registrationsByType.Keys;

	public static SerializerFactory Default { get; }

	public static void RegisterDefaults(SerializerFactory factory) {
		// primitives
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

		// nullables
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

		// other base .net types
		factory.Register(new StringSerializer().AsNullable());
		factory.Register(new GuidSerializer());
		factory.Register(new NullableStructSerializer<Guid>(new GuidSerializer()));

		// datetime
		factory.Register(new DateTimeSerializer());
		factory.Register(new NullableStructSerializer<DateTime>(new DateTimeSerializer()));
		factory.Register(new TimeSpanSerializer());
		factory.Register(new NullableStructSerializer<TimeSpan>(new TimeSpanSerializer()));
		

		// special collections
		factory.Register(new ByteArraySerializer());

		// general collections
		factory.Register(typeof(IEnumerable<>), typeof(EnumerableSerializer<>));
		//factory.Register(typeof(Array<>), typeof(ArraySerializer<>));
		factory.Register(typeof(ICollection<>), typeof(CollectionInterfaceSerializer<>));
		factory.Register(typeof(Collection<>), typeof(CollectionSerializer<>));
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>));
		factory.Register(typeof(List<>), typeof(ListSerializer<>));
		factory.Register(typeof(IDictionary<,>), typeof(DictionaryInterfaceSerializer<,>));
		factory.Register(typeof(Dictionary<,>), typeof(DictionarySerializer<,>));

	}

	#region Serializer hierarchy methods

	public RecursiveDataType<long> GetSerializerHierarchy(Type type) 
		=> _getSerializerHierarchyCache[type];

	public IItemSerializer FromSerializerHierarchy(RecursiveDataType<long> serializerHierarchy) 
		=> _fromSerializerHierarchyCache[serializerHierarchy];

	public long CountSubSerializers(long typeCode) {
		if (!_registrations.TryGetValue(typeCode, out var registration))
			throw new InvalidOperationException($"No serializer registered for type code {typeCode}");
		return registration.SerializerType.IsGenericTypeDefinition ? registration.SerializerType.GetGenericArguments().Length : 0;
	}

	#endregion

	#region Register

	public void Register<TItem>(IItemSerializer<TItem> serializerInstance)
		=> Register(GenerateTypeCode(), typeof(TItem), serializerInstance.GetType(), serializerInstance, null);

	public void Register<TItem, TSerializer>() where TSerializer : IItemSerializer<TItem>, new()
		=> Register(GenerateTypeCode(), typeof(TItem), typeof(TSerializer), null, _ => new TSerializer());

	public void Register<TItem>(Type serializerType) {
		Register(GenerateTypeCode(), typeof(TItem), serializerType, null, null);
	}

	public void Register(Type dataType, Type serializerType) {
		Register(GenerateTypeCode(), dataType, serializerType, null, null);
	}

	public void Register(long typeCode, Type dataType, Type serializerType, IItemSerializer serializerInstance, Func<Type, IItemSerializer> factory) {
		Guard.ArgumentNotNull(dataType, nameof(dataType));
		Guard.ArgumentNotNull(serializerType, nameof(serializerType));
		if (dataType.IsFullyConstructed() || serializerType.IsActivatable()) {
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

		// Ensure instance is of correct type
		if (serializerInstance != null) {
			if (!serializerType.IsInstanceOfType(serializerInstance))
				throw new InvalidOperationException($"Serializer instance is not of type {serializerType.Name}");

			Guard.Ensure(factory is null, "Factory must be null if serializer instance is provided");
			factory = _ => serializerInstance;
		} else if (factory is null) {
			// get all sub-types
			factory = requested => CreateSerializerInstance(requested, dataType, serializerType);
		}

		// add registration
		var registration = new Registration {
			TypeCode = typeCode,
			DataType = dataType,
			SerializerType = serializerType,
			Factory = factory,
		};
		_registrations.Add(typeCode, registration);
		_registrationsByType.Add(dataType, typeCode);

		_getSerializerCache.Flush();
		_fromSerializerHierarchyCache.Flush();
	}

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
		Guard.Argument(dataType.IsSubTypeOf(serializerDataType), nameof(dataType), $"{dataType.ToStringCS()} must be a sub-type of {serializerDataType.ToStringCS()}");
		var serializerObj = _getSerializerCache[dataType];

		var serializer = serializerObj as IItemSerializer<TSerializerDataType>;
		if (serializer is null) {
			var genericMethod = DecoratorExtensions.SerializerCastMethod.MakeGenericMethod(new [] { dataType,  serializerDataType });
			serializer = genericMethod.FastInvoke(null, new object[] { serializerObj }) as IItemSerializer<TSerializerDataType>;;
		}

		return serializer;
	}


	// Called by _getSerializerCache to get serializer
	private IItemSerializer GetSerializerInternal(Type dataType) {
		if (dataType.IsArray) {
			// Special Case, array serializers
			var valueSerializer = _getSerializerCache[dataType.GetElementType()];
			if (dataType == typeof(byte[]))
				return new ByteArraySerializer();
			return  (IItemSerializer)typeof(ArraySerializer<>).MakeGenericType(dataType).ActivateWithCompatibleArgs( new object [] { valueSerializer, SizeDescriptorStrategy.UseCVarInt } ) ;
		}
		Guard.ArgumentNotNull(dataType, nameof(dataType));
		var registration = FindCompatibleSerializer(dataType, out _);
		return registration.Factory(dataType);
	}

	#endregion

	#region Aux
	
	private bool TryFindCompatibleSerializer(Type dataType, out long typeCode, out Registration registration) {
		if (!_registrationsByType.TryGetValue(dataType, out typeCode))  {
			if (!dataType.IsGenericType || !_registrationsByType.TryGetValue(dataType.GetGenericTypeDefinition(), out typeCode)) {
				typeCode = -1;
				registration = default;
				return false;
			}
		}
		registration = _registrations[typeCode];
		return true;
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

	private long GenerateTypeCode() => _registrations.Count > 0 ? _registrations.Keys.Max() + 1 : 0;

	private IItemSerializer CreateSerializerInstance(Type requestedDataType, Type registeredDataType, Type registeredSerializerType) {
		Guard.Argument(!requestedDataType.IsGenericTypeDefinition, nameof(requestedDataType), $"Requested data type {requestedDataType.Name} cannot be a generic type definition");
		if (registeredDataType.IsGenericTypeDefinition)
			Guard.Ensure(requestedDataType.IsConstructedGenericTypeOf(registeredDataType), $"Constructed type {requestedDataType.Name} is not a constructed generic type of {registeredDataType.Name}");
		var subTypes = requestedDataType.GetGenericArguments();
		var subTypeSerializers = subTypes.Select(x => _getSerializerCache[x]).ToArray();
		var serializerType = registeredSerializerType;
		if (serializerType.IsGenericTypeDefinition)
			serializerType = serializerType.MakeGenericType(subTypes);
		return (IItemSerializer) TypeActivator.ActivateWithCompatibleArgs(serializerType, subTypeSerializers);
	}
		
	private Type ConstructType(RecursiveDataType<long> serializerHierarchy) {
		var registration = GetRegistration(serializerHierarchy.State);
		var dataType = registration.DataType;
		if (!dataType.IsGenericTypeDefinition)
			return dataType;
		var subTypes = serializerHierarchy.SubStates.Select(ConstructType).ToArray();
		return dataType.MakeGenericType(subTypes);
	}

	private RecursiveDataType<long> GetSerializerHierarchyInternal(Type type) {
		Guard.ArgumentNotNull(type, nameof(type));
		Guard.Against(type.IsGenericTypeDefinition, $"Type {type.Name} must be a generic type definition");
		var serializerRegistration = FindCompatibleSerializer(type, out var typeCode);
		var childSerializers = 
			serializerRegistration.SerializerType.IsGenericTypeDefinition ?
				type.GetGenericArguments().Select(GetSerializerHierarchy).ToArray() :
				Enumerable.Empty<RecursiveDataType<long>>();
		return new RecursiveDataType<long>(typeCode, childSerializers);
	}

	private IItemSerializer FromSerializerHierarchyInternal(RecursiveDataType<long> serializerHierarchy) {
		var registration = GetRegistration(serializerHierarchy.State);
		var type = ConstructType(serializerHierarchy);
		return registration.Factory(type);
	}

	#endregion

	public class Registration {
		public long TypeCode { get; set; }
		public Type DataType { get; set; }
		public Type SerializerType { get; set; }
		public Func<Type, IItemSerializer> Factory { get; set; }
	}
}
