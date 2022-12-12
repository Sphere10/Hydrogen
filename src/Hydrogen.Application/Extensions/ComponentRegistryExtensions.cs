//-----------------------------------------------------------------------
// <copyright file="ComponentRegistryExtensions.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Application {
	public static class ComponentRegistryExtensions {

        public static void RegisterInitializer<TApplicationInitializeTask>(this ComponentRegistry componentRegistry)
            where TApplicationInitializeTask : class, IApplicationInitializer {
            componentRegistry.RegisterComponent<IApplicationInitializer, TApplicationInitializeTask>(
                typeof(TApplicationInitializeTask).FullName
            );
        }

		public static void DeregisterInitializer<TApplicationInitializeTask>(this ComponentRegistry componentRegistry)
			where TApplicationInitializeTask : class, IApplicationInitializer {
			componentRegistry.DeregisterComponent<TApplicationInitializeTask>(
				typeof(TApplicationInitializeTask).FullName
			);
		}


		public static bool HasInitializer<TApplicationInitializeTask>(this ComponentRegistry componentRegistry) 
            where TApplicationInitializeTask : class, IApplicationInitializer {
            return componentRegistry.HasImplementationFor<IApplicationInitializer>(
                typeof(TApplicationInitializeTask).FullName
            );
        }

		public static void RegisterFinalizer<TApplicationFinalizer>(this ComponentRegistry componentRegistry)
            where TApplicationFinalizer : class, IApplicationFinalizer {
            componentRegistry.RegisterComponent<IApplicationFinalizer, TApplicationFinalizer>(
                typeof(TApplicationFinalizer).FullName
            );
        }

		public static void DeregisterFinalizer<TApplicationFinalizer>(this ComponentRegistry componentRegistry)
			where TApplicationFinalizer : class, IApplicationFinalizer {
			componentRegistry.DeregisterComponent<TApplicationFinalizer>(
				typeof(TApplicationFinalizer).FullName
			);
		}

        public static bool HasFinalizer<TApplicationFinalizer>(this ComponentRegistry componentRegistry) 
            where TApplicationFinalizer : class, IApplicationFinalizer {
            return componentRegistry.HasImplementationFor<TApplicationFinalizer>(
                typeof(TApplicationFinalizer).FullName
            );
        }

	}
}
