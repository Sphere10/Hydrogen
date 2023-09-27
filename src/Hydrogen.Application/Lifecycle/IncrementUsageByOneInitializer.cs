// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Application;

public class IncrementUsageByOneInitializer : ApplicationInitializerBase {

	public IncrementUsageByOneInitializer(IProductUsageServices productUsageServices) {
		ProductUsageServices = productUsageServices;
	}

	public IProductUsageServices ProductUsageServices { get; private set; }

	public override int Priority => 1;

	public override void Initialize() {
		ProductUsageServices.IncrementUsageByOne();
	}

}
