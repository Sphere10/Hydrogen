//-----------------------------------------------------------------------
// <copyright file="FastReflectionCaches.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Sphere10.Framework;

namespace Sphere10.Framework.FastReflection {

    public static class FastReflectionCaches {
        static FastReflectionCaches() {
            EnumNamesCache = new ActionCache<Type, string[]>( t => t.GetEnumNames());
            MethodInvokerCache = new ActionCache<MethodInfo, MethodInvoker>(mi => new MethodInvoker(mi));
            PropertyAccessorCache = new ActionCache<PropertyInfo, PropertyAccessor>(pi => new PropertyAccessor(pi));
            FieldAccessorCache = new ActionCache<FieldInfo, FieldAccessor>(fi => new FieldAccessor(fi));
            ConstructorInvokerCache = new ActionCache<ConstructorInfo, ConstructorInvoker>( ci => new ConstructorInvoker(ci));
        }

		public static ICache<Type, string[]> EnumNamesCache { get; set; }

        public static ICache<MethodInfo, MethodInvoker> MethodInvokerCache { get; set; }

        public static ICache<PropertyInfo, PropertyAccessor> PropertyAccessorCache { get; set; }

        public static ICache<FieldInfo, FieldAccessor> FieldAccessorCache { get; set; }

        public static ICache<ConstructorInfo, ConstructorInvoker> ConstructorInvokerCache { get; set; }

    }
}
