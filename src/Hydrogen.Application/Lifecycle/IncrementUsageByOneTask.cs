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

namespace Hydrogen.Application;



public class IncrementUsageByOneTask : BaseApplicationInitializer {

	public IncrementUsageByOneTask(IProductUsageServices productUsageServices) {
		ProductUsageServices = productUsageServices;
	}

	public IProductUsageServices ProductUsageServices { get; private set; }

	public override int Priority => 1;

	public override void Initialize() {
		ProductUsageServices.IncrementUsageByOne();
	}

}
