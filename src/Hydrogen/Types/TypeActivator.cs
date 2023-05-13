// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;


namespace Hydrogen {
	public static class TypeActivator {

        public static object Create(string typeName, string assemblyHint, params object[] parameters) { 
            // resolve the type
            var targetType = TypeResolver.Resolve(typeName, assemblyHint);
            return Create(targetType, parameters);
        }

        public static object Create(string typeName, params object[] parameters) {
            // resolve the type
            var targetType = TypeResolver.Resolve(typeName);
            return Create(targetType, parameters);
        }

        public static object Create(Type targetType) {
            return Create(targetType, new object[0]);
        }

        public static object Create(Type targetType, params object[] parameters) {

            // get the default constructor and instantiate
            var types = parameters?.Where(x => x != null).Select(x => x.GetType()).ToArray() ?? new Type[0];
            var info = targetType.GetConstructor(types);
            if (info == null)
                throw new ArgumentException("Can't instantiate type " + targetType.FullName);
#if USE_FAST_REFLECTION
		    var targetObject = info.FastInvoke(parameters);		// using FastReflectionLib
#else
            var targetObject = info.Invoke(parameters); // using standard Reflection
#endif
            if (targetObject == null)
                throw new ArgumentException("Can't instantiate type " + targetType.FullName);

            return targetObject;
        }



    }
}


