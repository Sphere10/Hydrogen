using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Framework {
	public abstract class GenericItemSerializer {

		protected static readonly SynchronizedDictionary<Type, int> Registrations = new();

		protected static readonly SynchronizedDictionary<Type, IItemSerializer<object>> Serializers = new();

		public GenericItemSerializer() {
			// special type headers
			Register(typeof(NullValue));
			Register(typeof(CircularReference));

			// built-in .NET types registered, no custom serializer required.
			Register(typeof(Nullable<>));
			Register(typeof(Dictionary<,>));
			Register(typeof(List<>));
			Register(typeof(ArrayList));
			Register(typeof(Array));
			Register(typeof(object));
			Register(typeof(string));
			Register(typeof(int));
			Register(typeof(char));
			Register(typeof(decimal));
			Register(typeof(double));
			Register(typeof(bool));
			Register(typeof(byte));
			
			Register(new DateTimeSerializer());
			Register(new DateTimeOffsetSerializer());
			Register(new DecimalSerializer());
			Register(new StringSerializer());
		}

		public static void Register<T>() where T : new() => Register<T>(Registrations.Count + 1);

		public static void Register<T>(int typeCode) where T : new() => Register(typeof(T), typeCode);

		public static void Register<T>(IItemSerializer<T> serializer) => Register(serializer, Registrations.Count + 1);

		public static void Register(Type type, IItemSerializer<object> serializer) => Register(type, serializer, Registrations.Count + 1);

		public static void Register(Type type) => Register(type, Registrations.Count + 1);

		public static void Register<T>(IItemSerializer<T> serializer, int typeCode) => Register(typeof(T), serializer.AsBaseSerializer<T, object>(), typeCode);

		protected static void Register(Type type, IItemSerializer<object> serializer, int typeCode) {
			Registrations.TryAdd(type, typeCode);
			Serializers.TryAdd(type, serializer);
		}

		protected static void Register(Type type, int typecode) {
			Registrations.TryAdd(type, typecode);
		}


		protected struct NullValue {
		}


		protected struct CircularReference {
			public int TypeIndex { get; set; }

			public ushort Index { get; set; }
		}


		protected int GetTypeIndex(Type type) {
			return Registrations.ContainsKey(type) ? Registrations[type] : throw new InvalidOperationException($"Type {type.Name} is not registered, add using Register<T>()");
		}
	}
}
