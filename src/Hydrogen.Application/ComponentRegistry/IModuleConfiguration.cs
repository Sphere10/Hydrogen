//-----------------------------------------------------------------------
// <copyright file="IModuleConfiguration.cs" company="Sphere 10 Software">
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
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Application {
	public interface IModuleConfiguration {
		int Priority { get; }
        void RegisterComponents(IServiceCollection services);
        void OnInitialize(IServiceProvider serviceProvider);
        void OnFinalize(IServiceProvider serviceProvider);
        
    }
}
