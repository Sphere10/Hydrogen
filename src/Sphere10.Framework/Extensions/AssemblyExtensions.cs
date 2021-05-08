//-----------------------------------------------------------------------
// <copyright file="AssemblyExtensions.cs" company="Sphere 10 Software">
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
using System.Reflection;

namespace Sphere10.Framework {

	public static class AssemblyExtensions {
		static readonly string[] FrameworkPrefixes = new[]{
			"mscorlib",
			"System,",
			"System.",
			"Accessibility,",
			"netstandard,",
			"PresentationCore,",
			"WindowsBase,",
			"App_global.asax.",
			"Microsoft.",
			"SMDiagnostics,",
		};

		public static IEnumerable<Assembly> GetNonFrameworkAssemblies(this AppDomain domain) {
			return domain.GetAssemblies().Where(x =>  !FrameworkPrefixes.Any(p => x.FullName.StartsWith(p)));
		}
        public static IEnumerable<Type> GetDerivedTypes<T>(this Assembly assembly) {
            return assembly.GetDerivedTypes(typeof(T));
        }

        public static IEnumerable<Type> GetDerivedTypes(this Assembly assembly, Type baseType) {
            return assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));
        }
    }
}
