// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hydrogen.FastReflection;

namespace Hydrogen;

public static class TypeActivator {

	public static object Activate(string typeName, string assemblyHint, params object[] parameters) {
		// resolve the type
		var targetType = TypeResolver.Resolve(typeName, assemblyHint);
		return Activate(targetType, parameters);
	}

	public static object Activate(string typeName, params object[] parameters) {
		// resolve the type
		var targetType = TypeResolver.Resolve(typeName);
		return Activate(targetType, parameters);
	}

	public static T Activate<T>() => (T)Activate(typeof(T));

	public static object Activate(Type targetType) => Activate(targetType, Array.Empty<object>());

	public static T Activate<T>(params object[] args) => (T)Activate(typeof(T), args);

	public static object Activate(Type targetType, params object[] parameters) {
		// get the default constructor and instantiate
		var types = parameters?.Where(x => x != null).Select(x => x.GetType()).ToArray() ?? new Type[0];
		var constructor = targetType.GetConstructor(types);
		if (constructor == null)
			throw new ArgumentException("Can't instantiate type " + targetType.FullName);

#if USE_FAST_REFLECTION
	    var targetObject = info.FastInvoke(parameters);		// using FastReflectionLib
#else
		var targetObject = constructor.Invoke(parameters); // using standard Reflection
#endif

		if (targetObject == null)
			throw new ArgumentException("Can't instantiate type " + targetType.FullName);

		return targetObject;
	}

	public static ConstructorInfo FindCompatibleConstructor(Type type, Type[] parameterTypes) {
		Guard.ArgumentNotNull(type, nameof(type));
		Guard.ArgumentNotNull(parameterTypes, nameof(parameterTypes));

		return type
			.GetConstructors()
			.FirstOrDefault(constructor => {
				var parameters = constructor.GetParameters();

				// Create a list of the provided parameter types for manipulation.
				var unmatchedTypes = parameterTypes.ToList();
				foreach (var param in parameters) {
					var matchingType = unmatchedTypes.FirstOrDefault(t => param.ParameterType.IsAssignableFrom(t));

					if (matchingType != null) {
						// Remove the matched type so it's not considered again.
						unmatchedTypes.Remove(matchingType);
					} else if (!param.HasDefaultValue) {
						// If the parameter is not matched and does not have a default value, the constructor is not a match.
						return false;
					}
				}

				// If there are any unmatched provided types left, it's not a match.
				if (unmatchedTypes.Any()) 
					return false;

				// If we've made it this far, it's a match.
				return true;
			});
	}

	public static bool TryActivateWithCompatibleArgs(Type type, object[] args, out object instance) {
		Guard.ArgumentNotNull(type, nameof(type));
		Guard.ArgumentNotNull(args, nameof(args));
		Guard.Ensure(args.All(a => a != null), "All arguments must be non-null");

		var constructor = FindCompatibleConstructor(type, args.Select(a => a.GetType()).ToArray());
		if (constructor == null) {
			instance = null;
			return false;
		}

		// Determine the correct order of arguments.
		var constructorParameters = constructor.GetParameters();
		var orderedArgs = new object[constructorParameters.Length];
		var usedIndexes = new HashSet<int>(); // To keep track of already used arguments

		for (var i = 0; i < constructorParameters.Length; i++) {
			for (var j = 0; j < args.Length; j++) {
				// Check if the argument has not been used and is of the correct type
				if (!usedIndexes.Contains(j) && constructorParameters[i].ParameterType.IsInstanceOfType(args[j])) {
					orderedArgs[i] = args[j];
					usedIndexes.Add(j); // Mark this argument index as used
					break;
				}
			}

			// If no argument is found, and the parameter has a default value, use that.
			if (orderedArgs[i] == null && constructorParameters[i].HasDefaultValue) {
				orderedArgs[i] = constructorParameters[i].DefaultValue;
			}
		}

#if USE_FAST_REFLECTION
		instance = constructor.FastInvoke(orderedArgs);
#else
		instance = constructor.Invoke(orderedArgs); // using standard Reflection
#endif
		return true;
	}

//	public static bool TryActivateWithCompatibleArgs(Type type, object[] args, out object instance) {
//		Guard.ArgumentNotNull(type, nameof(type));
//		Guard.ArgumentNotNull(args, nameof(args));
//		Guard.Ensure(args.All(a => a != null), "All arguments must be non-null");

//		var constructor = FindCompatibleConstructor(type, args.Select(a => a.GetType()).ToArray());
//		if (constructor == null) {
//			instance = null;
//			return false;
//		}

//		// Determine the correct order of arguments.
//		var constructorParameters = constructor.GetParameters();
//		var orderedArgs = new object[constructorParameters.Length];

//		for (var i = 0; i < constructorParameters.Length; i++) {
//			// Try to find a matching argument.
//			foreach (var arg in args) {
//				if (!constructorParameters[i].ParameterType.IsInstanceOfType(arg))
//					continue;
//				orderedArgs[i] = arg;
//				break;
//			}

//			// If no argument is found, and the parameter has a default value, use that.
//			if (orderedArgs[i] == null && constructorParameters[i].HasDefaultValue) {
//				orderedArgs[i] = constructorParameters[i].DefaultValue;
//			}
//		}

//#if USE_FAST_REFLECTION
//		instance = constructor.Invoke(orderedArgs);
//#else
//		instance = constructor.Invoke(orderedArgs); // using standard Reflection
//#endif
//		return true;
//	}


	public static T ActivateWithCompatibleArgs<T>(Type type, object[] args) 
		=> (T)ActivateWithCompatibleArgs(type, args);

	public static object ActivateWithCompatibleArgs(Type type, object[] args) {
		if (!TryActivateWithCompatibleArgs(type, args, out var instance))
			throw new InvalidOperationException($"No compatible constructor was found for type {type.ToStringCS()} for args [{args.Select(x => x is not null ? x.GetType().ToStringCS() : "NULL").ToDelimittedString(", ")}].");
		return instance;
	}

}
