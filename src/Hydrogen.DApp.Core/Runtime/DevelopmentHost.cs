// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;

namespace Hydrogen.DApp.Core.Runtime;

public class DevelopmentHost : Host {

	public DevelopmentHost(ILogger logger)
		: this(logger, new DevelopmentApplicationPaths()) {
	}

	public DevelopmentHost(ILogger logger, IApplicationPaths paths)
		: base(logger, paths) {
	}

	public override Task DeployHAP(string newHapPath) {
		throw new NotSupportedException("Deployment is not supported in development mode");
	}

}
