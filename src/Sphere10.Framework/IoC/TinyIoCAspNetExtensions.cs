//-----------------------------------------------------------------------
// <copyright file="TinyIoCAspNetExtensions.cs" company="Sphere 10 Software">
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
using Sphere10.Framework;

namespace TinyIoC {
	public static class TinyIoCAspNetExtensions {

		static TinyIoCAspNetExtensions() {
			HttpContextLifetimeProviderType = TypeResolver.Resolve("HttpContextLifetimeProvider");
		}
		private static Type HttpContextLifetimeProviderType { get; set; }
		public static TinyIoCContainer.RegisterOptions AsPerRequestSingleton(this TinyIoCContainer.RegisterOptions registerOptions) {
			throw new NotImplementedException();
			//var httpContextLifetimeProvider = (TinyIoCContainer.ITinyIoCObjectLifetimeProvider)TypeActivator.Create(HttpContextLifetimeProviderType);
			//return TinyIoCContainer.RegisterOptions.ToCustomLifetimeManager(registerOptions, httpContextLifetimeProvider, "per request singleton");
		}
	}
}
