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
using System.Reflection;
using System.Xml.Schema;

namespace Hydrogen.FastReflection {

	public static class FastReflectionCaches {
        static FastReflectionCaches() {
            EnumNamesCache = new ActionCache<Type, string[]>( t => t.GetEnumNames());
            MethodInvokerCache = new ActionCache<MethodInfo, MethodInvoker>(mi => new MethodInvoker(mi), keyComparer: new MemberInfoComparer<MethodInfo>());
            PropertyAccessorCache = new ActionCache<PropertyInfo, PropertyAccessor>(pi => new PropertyAccessor(pi), keyComparer: new MemberInfoComparer<PropertyInfo>());
            FieldAccessorCache = new ActionCache<FieldInfo, FieldAccessor>(fi => new FieldAccessor(fi), keyComparer: new MemberInfoComparer<FieldInfo>());
            ConstructorInvokerCache = new ActionCache<ConstructorInfo, ConstructorInvoker>( ci => new ConstructorInvoker(ci), keyComparer: new MemberInfoComparer<ConstructorInfo>());
        }

		public static ICache<Type, string[]> EnumNamesCache { get; set; }

        public static ICache<MethodInfo, MethodInvoker> MethodInvokerCache { get; set; }

        public static ICache<PropertyInfo, PropertyAccessor> PropertyAccessorCache { get; set; }

        public static ICache<FieldInfo, FieldAccessor> FieldAccessorCache { get; set; }

        public static ICache<ConstructorInfo, ConstructorInvoker> ConstructorInvokerCache { get; set; }

    }

}
