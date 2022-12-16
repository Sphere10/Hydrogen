//-----------------------------------------------------------------------
// <copyright file="IncrementUsageByOneTask.cs" company="Sphere 10 Software">
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

using Hydrogen.Application;
using Microsoft.Extensions.Hosting;

namespace Hydrogen.Web.AspNetCore;

internal class AspNetCoreLifecycleMonitorInitializer : ApplicationInitializerBase {

	public AspNetCoreLifecycleMonitorInitializer(IHostApplicationLifetime hostApplicationLifetime) {
		HostApplicationLifetime = hostApplicationLifetime;
	}
	protected IHostApplicationLifetime HostApplicationLifetime { get; }

	public override void Initialize() {
		HostApplicationLifetime.ApplicationStopped.Register(HydrogenFramework.Instance.EndFramework);
	}

}
