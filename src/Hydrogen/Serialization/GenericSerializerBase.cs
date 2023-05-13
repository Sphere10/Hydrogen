// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hydrogen {
	public abstract class GenericSerializerBase {

		protected readonly SynchronizedDictionary<Type, int> Registrations = new();

		protected readonly SynchronizedDictionary<Type, IItemSerializer<object>> Serializers = new();

		public GenericSerializerBase() {
			
			RegisterType<NullValue>();
			RegisterType<CircularReference>();
			
			RegisterSerializer(new DateTimeSerializer());
			RegisterSerializer(new DateTimeOffsetSerializer());
			RegisterSerializer(new PrimitiveSerializer<decimal>());
			RegisterSerializer(new StringSerializer(Encoding.UTF8));
			RegisterSerializer(new ByteArraySerializer());
		}

		/// <summary>
		/// Registers the type with the serializer. Required to serialize and deserialize types.
		/// </summary>
		/// <typeparam name="T"> type to register.</typeparam>
		public void RegisterType<T>() => RegisterType(typeof(T));

		/// <summary>
		/// Registers the type with the serializer. Required to serialize and deserialize types.
		/// </summary>
		/// <param name="type"> type to register</param>
		public void RegisterType(Type type) => RegisterTypeInternal(type);

		public void RegisterSerializer<T>(IItemSerializer<T> serializer) 
			=> RegisterSerializer(typeof(T), serializer.AsBaseSerializer<T, object>());

		protected void RegisterSerializer(Type type, IItemSerializer<object> serializer) {
			RegisterType(type);
			Serializers.TryAdd(type, serializer);
		}

		/// <summary>
		/// Gets the type code for type. If the type has not been registered, it is added to known type
		/// registrations.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected int DetermineTypeCode(Type type) {
			if (Registrations.ContainsKey(type))
				return Registrations[type];
			else {
				var code = CreateTypeCodeForType(type);
				Registrations.Add(type, code);
				return code;
			}
		}
		
		private int CreateTypeCodeForType(Type type) {
			// TODO: add support for injected type code definitions
			// TODO: add support for attribute based type code definitions
			var nameHash = Hashers.Hash(CHF.SHA2_256, Encoding.UTF8.GetBytes(type.AssemblyQualifiedName));
			return EndianBitConverter.Little.ToInt32(nameHash[..4]);
		}

		/// <summary>
		/// Registers the type and its properties' types, recursively. 
		/// </summary>
		/// <param name="type"></param>
		private void RegisterTypeInternal(Type type) {
			if (type.IsGenericType) {
				var genericDef = type.GetGenericTypeDefinition();
				if (!Registrations.ContainsKey(genericDef))
					Registrations.TryAdd(genericDef, CreateTypeCodeForType(genericDef));

				foreach (var argument in type.GetGenericArguments()) {
					if (!Registrations.ContainsKey(argument))
						Registrations.TryAdd(argument, CreateTypeCodeForType(argument));
				}
					
			} else if (type.IsArray) {
				if (!Registrations.ContainsKey(typeof(Array)))
					Registrations.TryAdd(typeof(Array), CreateTypeCodeForType(typeof(Array)));

				var elementType = type.GetElementType();
				if (!Registrations.ContainsKey(elementType))
					Registrations.TryAdd(elementType, CreateTypeCodeForType(elementType));
			} else {
				if (!Registrations.ContainsKey(type)) {
					Registrations.TryAdd(type, CreateTypeCodeForType(type));
				}
			}
			
			foreach (var property in GetSerializableProperties(type)) {
				if (!Registrations.ContainsKey(property.PropertyType))
					RegisterTypeInternal(property.PropertyType);
			}
		}

		/// <summary>
		/// Get <see cref="PropertyInfo"/> of a given type that can be serialized. 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected PropertyInfo[] GetSerializableProperties(Type type) {
			return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
				.Where(x => x.CanRead && x.CanWrite)
				.ToArray();
		}


		/// <summary>
		/// Internal struct used by the generic serializer to indicate null. 
		/// </summary>
		protected struct NullValue {
		}

		/// <summary>
		/// Internal struct used by the generic serializer to indicate a circular reference
		/// </summary>
		protected struct CircularReference {
			public int TypeCode { get; set; }

			public ushort Index { get; set; }
		}

	}
}
