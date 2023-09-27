// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.Application;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceProviderNamedRegistrationsExtensions {

	public static T GetNamedService<T>(this IServiceProvider servicerProvider, string name) where T : class {
		var namedLookup = servicerProvider.GetService<INamedLookup<T>>();
		return namedLookup?[name];
	}

}
