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
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Windows.Forms {
	public static class IoCExtensions {

		public static void AddMainForm<TMainForm>(this IServiceCollection serviceCollection)
			where TMainForm : class, IMainForm {
			serviceCollection.AddSingleton<IMainForm, TMainForm>();
			serviceCollection.AddSingleton<IApplicationIconProvider>( provider => provider.GetService<IMainForm>());
			serviceCollection.AddSingleton<IUserInterfaceServices>( provider => provider.GetService<IMainForm>());
			if (typeof(TMainForm).IsSubclassOf(typeof(IBlockManager))) 
				serviceCollection.AddSingleton(provider => (IBlockManager)provider.GetService<IMainForm>());
		}

		public static void AddApplicationBlock<T>(this IServiceCollection serviceCollection) where T : class, IApplicationBlock 
			=> serviceCollection.AddTransient<IApplicationBlock, T>();

		public static void AddControlStateEventProvider<TControl, TProvider>(this IServiceCollection servicesCollection)
			where TControl : Control
			where TProvider : class, IControlStateEventProvider {
			var controlType = typeof(TControl);
			servicesCollection.AddNamedTransient<IControlStateEventProvider, TProvider>(controlType.FullName);
		}


		public static bool HasControlStateEventProvider<TControl>(this IServiceCollection servicesCollection) 
			=> servicesCollection.HasNamedImplementationFor<IControlStateEventProvider>(typeof(TControl).FullName);
		

		public static IControlStateEventProvider GetControlStateEventProvider(this IServiceProvider serviceProvider, Control control)
			=> GetControlStateEventProvider(serviceProvider, control.GetType());

		public static IControlStateEventProvider GetControlStateEventProvider(this IServiceProvider serviceProvider, Type controlType) {
			var namedLookup = serviceProvider.GetService<INamedLookup<IControlStateEventProvider>>();
			return namedLookup?[controlType.FullName];
		}
	}
}
