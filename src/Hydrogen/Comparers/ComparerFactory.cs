using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hydrogen.Collections;
using static Hydrogen.SerializerFactory;

namespace Hydrogen;
public class ComparerFactory {
	private IDictionary<Type, object> _equalityComparers;
	private IDictionary<Type, object> _comparers;

	static ComparerFactory() {
		Default = new ComparerFactory();
		RegisterDefaults(Default);
	}

	public ComparerFactory() {
		_equalityComparers = new Dictionary<Type, object>();
		_comparers = new Dictionary<Type, object>();
	}

	public ComparerFactory(ComparerFactory comparerFactory) : this() {
		CopyRegistrations(comparerFactory);
	}

	public void CopyRegistrations(ComparerFactory factory) {
		Guard.ArgumentNotNull(factory, nameof(factory));
		_comparers = factory._comparers.ToDictionary();
		_equalityComparers = factory._equalityComparers.ToDictionary();
	}

	public static ComparerFactory Default { get; }

	public static void RegisterDefaults(ComparerFactory comparerFactory) {

		#region IComparer
		comparerFactory.RegisterComparer(Comparer<bool>.Default);
		comparerFactory.RegisterComparer(Comparer<sbyte>.Default);
		comparerFactory.RegisterComparer(Comparer<byte>.Default);
		comparerFactory.RegisterComparer(Comparer<char>.Default);
		comparerFactory.RegisterComparer(Comparer<ushort>.Default);
		comparerFactory.RegisterComparer(Comparer<short>.Default);
		comparerFactory.RegisterComparer(Comparer<uint>.Default);
		comparerFactory.RegisterComparer(Comparer<int>.Default);
		comparerFactory.RegisterComparer(Comparer<ulong>.Default);
		comparerFactory.RegisterComparer(Comparer<long>.Default);
		comparerFactory.RegisterComparer(Comparer<float>.Default);
		comparerFactory.RegisterComparer(Comparer<double>.Default);
		comparerFactory.RegisterComparer(Comparer<decimal>.Default);
		comparerFactory.RegisterComparer(Comparer<DateTime>.Default);
		comparerFactory.RegisterComparer(Comparer<TimeSpan>.Default);
		comparerFactory.RegisterComparer(Comparer<DateTimeOffset>.Default);
		comparerFactory.RegisterComparer(Comparer<Guid>.Default);

		// Nullables
		comparerFactory.RegisterComparer(Comparer<bool?>.Default);
		comparerFactory.RegisterComparer(Comparer<sbyte?>.Default);
		comparerFactory.RegisterComparer(Comparer<byte?>.Default);
		comparerFactory.RegisterComparer(Comparer<char?>.Default);
		comparerFactory.RegisterComparer(Comparer<ushort?>.Default);
		comparerFactory.RegisterComparer(Comparer<short?>.Default);
		comparerFactory.RegisterComparer(Comparer<uint?>.Default);
		comparerFactory.RegisterComparer(Comparer<int?>.Default);
		comparerFactory.RegisterComparer(Comparer<ulong?>.Default);
		comparerFactory.RegisterComparer(Comparer<long?>.Default);
		comparerFactory.RegisterComparer(Comparer<float?>.Default);
		comparerFactory.RegisterComparer(Comparer<double?>.Default);
		comparerFactory.RegisterComparer(Comparer<decimal?>.Default);
		comparerFactory.RegisterComparer(Comparer<DateTime?>.Default);
		comparerFactory.RegisterComparer(Comparer<TimeSpan?>.Default);
		comparerFactory.RegisterComparer(Comparer<DateTimeOffset?>.Default);
		comparerFactory.RegisterComparer(Comparer<Guid?>.Default);

		// other base types
		comparerFactory.RegisterComparer(Comparer<string>.Default);
		comparerFactory.RegisterComparer(Comparer<byte[]>.Default);
		#endregion

		#region IEqualityComparer
		comparerFactory.RegisterEqualityComparer(EqualityComparer<bool>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<sbyte>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<byte>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<char>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<ushort>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<short>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<uint>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<int>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<ulong>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<long>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<float>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<double>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<decimal>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<DateTime>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<TimeSpan>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<DateTimeOffset>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<Guid>.Default);

		// Nullables
		comparerFactory.RegisterEqualityComparer(EqualityComparer<bool?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<sbyte?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<byte?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<char?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<ushort?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<short?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<uint?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<int?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<ulong?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<long?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<float?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<double?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<decimal?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<DateTime?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<TimeSpan?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<DateTimeOffset?>.Default);
		comparerFactory.RegisterEqualityComparer(EqualityComparer<Guid?>.Default);

		// other base types
		comparerFactory.RegisterEqualityComparer(EqualityComparer<string>.Default);
		comparerFactory.RegisterEqualityComparer(new ByteArrayEqualityComparer());
		#endregion

	}
	
	public IEqualityComparer<string> GetEqualityComparer<T>()
		=> (IEqualityComparer<string>)GetEqualityComparer(typeof(T));

	public object GetEqualityComparer(Type type) {
		// Get registered comparer
		if (_equalityComparers.TryGetValue(type, out var comparer))
			return comparer;

		// Special case comparer: array
		if (type.IsArray) {
			var elementType = type.GetElementType();
			var arrayComparerType = typeof(ArrayEqualityComparer<>).MakeGenericType(elementType);
			return Activator.CreateInstance(arrayComparerType);
		}

		// Special case comparer: list
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
			var elementType = type.GetGenericArguments()[0];
			var listComparerType = typeof(ListEqualityComparer<>).MakeGenericType(elementType);
			return Activator.CreateInstance(listComparerType);
		}

		// Special case comparer: extended list
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
			var elementType = type.GetGenericArguments()[0];
			var listComparerType = typeof(ExtendedListEqualityComparer<>).MakeGenericType(elementType);
			return Activator.CreateInstance(listComparerType);
		}

		// Special case comparer: dictionary
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
			var keyType = type.GetGenericArguments()[0];
			var valueType = type.GetGenericArguments()[1];
			var dictionaryComparerType = typeof(DictionaryEqualityComparer<,>).MakeGenericType(keyType, valueType);
			return Activator.CreateInstance(dictionaryComparerType);
		}

		// Special case comparer: KeyValuePair
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)) {
			var keyType = type.GetGenericArguments()[0];
			var valueType = type.GetGenericArguments()[1];
			var kvpComparerType = typeof(KeyValuePairEqualityComparer<,>).MakeGenericType(keyType, valueType);
			return Activator.CreateInstance(kvpComparerType);
		}

		// Default: use EqualityComparer<T>.Default
		var defaultComparer = typeof(EqualityComparer<>)
			.MakeGenericType(type)
			.GetMethod("Default", BindingFlags.Static | BindingFlags.Public)
			.Invoke(null, null);

		return defaultComparer;
	}
	
	public IComparer<string> GetComparer<T>()
		=> (IComparer<string>)GetComparer(typeof(T));

	public object GetComparer(Type type) {
			// Get registered comparer
		if (_equalityComparers.TryGetValue(type, out var comparer))
			return comparer;

		// Default: use EqualityComparer<T>.Default
		var defaultComparer = typeof(Comparer<>)
			.MakeGenericType(type)
			.GetMethod("Default", BindingFlags.Static | BindingFlags.Public)
			.Invoke(null, null);

		return defaultComparer;
	}
		

	public void RegisterComparer<T>(IComparer<T> comparer)
		=> _comparers.Add(typeof(T), comparer);

	public void RegisterEqualityComparer<T>(IEqualityComparer<T> equalityComparer)
		=> _equalityComparers.Add(typeof(T), equalityComparer);

}
