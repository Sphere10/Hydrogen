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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sphere10.Framework.Application;

namespace Sphere10.Framework.Application {
    public abstract class ModuleConfigurationBase : IModuleConfiguration {
		public virtual int Priority => 0;

	    public virtual void RegisterComponents(ComponentRegistry registry) {
        }

        public virtual void OnApplicationStart() {            
        }

        public virtual void OnApplicationEnd() {
            
        }

    }
}
