//-----------------------------------------------------------------------
// <copyright file="ModuleConfiguration.cs" company="Sphere 10 Software">
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
using Sphere10.Framework;
using Sphere10.Framework.Application;

namespace Sphere10.Hydrogen.Node {
	public class ModuleConfiguration : ModuleConfigurationBase {

		public override void RegisterComponents(ComponentRegistry registry) {
			// Init tasks
			registry.RegisterInitializationTask<HydrogenInitializer>();


			// Start Tasks
			registry.RegisterStartTask<StartNodeTask>();


			// End Tasks

		}


		public override void DeregisterComponents(ComponentRegistry registry) {
			registry.DeregisterInitializationTask<HydrogenInitializer>();
			registry.DeregisterStartTask<StartNodeTask>();
		}

	}
}
