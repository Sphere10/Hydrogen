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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen;
using Hydrogen.Application;

namespace Hydrogen.Windows.Forms {
    public static class ComponentRegistryExtensions {
        
        internal static readonly IDictionary<Type, int> BlockPositions;

        static ComponentRegistryExtensions() {
            BlockPositions = new Dictionary<Type, int>();
        }

        public static void RegisterApplicationBlock<T>(this ComponentRegistry componentRegistry, int sequence) where T : class, IApplicationBlock {
            BlockPositions.Add(typeof(T), sequence);
            componentRegistry.RegisterComponent<IApplicationBlock, T>(typeof(T).FullName);
        }

        public static void RegisterMainForm<TMainForm>(this ComponentRegistry componentRegistry)
            where TMainForm : class, IMainForm {
            componentRegistry.RegisterComponent<IMainForm, TMainForm>(ActivationType.Singleton);
            componentRegistry.RegisterProxyComponent<IApplicationIconProvider, IMainForm>();
            componentRegistry.RegisterProxyComponent<IUserInterfaceServices, IMainForm>();
            componentRegistry.RegisterProxyComponent<IUserNotificationServices, IMainForm>();
            if (typeof(TMainForm).IsSubclassOf(typeof(IBlockManager))) {
                //RegisterComponentInstance<BlockMainForm>(mainForm as BlockMainForm);
                componentRegistry.RegisterProxyComponent<IBlockManager, IMainForm>();
            }
        }

        public static void RegisterControlStateEventProvider<TControl, TProvider>(this ComponentRegistry componentRegistry)
            where TControl : Control
	        where TProvider : class, IControlStateEventProvider {
            var controlType = typeof(TControl);		 
            ComponentRegistry.Instance.RegisterComponent<IControlStateEventProvider, TProvider>(controlType.FullName);
        }

        public static IControlStateEventProvider ResolveControlStateEventProvider(this ComponentRegistry componentRegistry, Control control)
	        => ResolveControlStateEventProvider(componentRegistry, control.GetType());

        public static IControlStateEventProvider ResolveControlStateEventProvider(this ComponentRegistry componentRegistry, Type controlType) {
	        return ComponentRegistry.Instance.Resolve<IControlStateEventProvider>(controlType.FullName);
        }
    }
}
