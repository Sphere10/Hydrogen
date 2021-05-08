//-----------------------------------------------------------------------
// <copyright file="ModuleConfigurationBase.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Application {
	public abstract class ModuleConfigurationBase : IModuleConfiguration {
		public virtual int Priority => 0;

	    public virtual void RegisterComponents(ComponentRegistry registry) {
        }

		public virtual void DeregisterComponents(ComponentRegistry registry) {
		}

		public virtual void OnInitialize() {            
        }

        public virtual void OnFinalize() {
            
        }

    }
}
