using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Sphere10.Framework {
	public abstract class GenericItemSerializer {
		
		protected static readonly SynchronizedDictionary<Type, int> Registrations = new ();
		
		protected static readonly SynchronizedDictionary<Type, IItemSerializer<object>> Serializers = new ();
		
		public GenericItemSerializer() {
			
			// special type headers
			Registrations.TryAdd(typeof(NullValue), Registrations.Count + 1);
			Registrations.TryAdd(typeof(CircularReference), Registrations.Count + 1);
			
			// built-in .NET types registered, no custom serializer required.
			Registrations.TryAdd(typeof(Dictionary<,>), Registrations.Count + 1);
			Registrations.TryAdd(typeof(List<>), Registrations.Count + 1);
			Registrations.TryAdd(typeof(ArrayList), Registrations.Count + 1);
			Registrations.TryAdd(typeof(Array), Registrations.Count + 1);
			Registrations.TryAdd(typeof(object), Registrations.Count + 1);
			Registrations.TryAdd(typeof(string), Registrations.Count + 1);
			Registrations.TryAdd(typeof(int), Registrations.Count + 1);
			Registrations.TryAdd(typeof(decimal), Registrations.Count + 1);
			Registrations.TryAdd(typeof(double), Registrations.Count + 1);
			
			Register(new DateTimeSerializer());
			Register(new DateTimeOffsetSerializer());
		}
		
		public static void Register<T>() where T : new() => Register<T>(Registrations.Count + 1);

		public static void Register<T>(int typeCode) where T : new() => Register(GenericItemSerializer<T>.Default
			, typeCode);

		public static void Register<T>(IItemSerializer<T> serializer) where T : new() => Register(serializer, Registrations.Count + 1);

		public static void Register(Type type, IItemSerializer<object> serializer) => Register(type, serializer, Registrations.Count + 1);
		
		public static void Register<T>(IItemSerializer<T> serializer, int typeCode) => Register(typeof(T), serializer.AsBaseSerializer<T, object>(), typeCode);

		protected static void Register(Type type, IItemSerializer<object> serializer, int typeCode) {
			Registrations.TryAdd(type, typeCode);
			Serializers.TryAdd(type, serializer);
		}
		
		protected struct NullValue {
		}
		
		protected struct CircularReference {
		}
	}
}
