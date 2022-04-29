//-----------------------------------------------------------------------
// <copyright file="iOSChannelRegister.cs" company="Sphere 10 Software">
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

using System.Linq;
#if __IOS__
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Sphere10.Framework.Services {
	public static class iOSChannelRegister {
		private static readonly IDictionary<Type, Func<Binding,EndpointAddress, object>> _registeredClients;

		static iOSChannelRegister() {
			_registeredClients = new Dictionary<Type, Func<Binding,EndpointAddress, object>>();
		}


		public static bool ContainsClientFor<T>() {
			return _registeredClients.ContainsKey(typeof(T));
		}

		public static T CreateClient<T>(Binding binding, EndpointAddress address) where T : class {
			if (!ContainsClientFor<T>())
			    throw new SoftwareException("No channel is registered for type '{0}'", typeof(T));

			var registeredClientConstructor = _registeredClients[typeof(T)];
		    var client = registeredClientConstructor(binding, address) as iOSClientBase<T>;
            if (client == null)
                throw new SoftwareException("Registered constructor did not ceate an iOSClientBase<{0}> instance", typeof(T).Name);

            return client.Channel;
		}

		public static void RegisterChannel<TInterface, TImplementation>(Func<Binding, EndpointAddress,TImplementation> constructor) where TInterface : class where  TImplementation : iOSClientBase<TInterface> {
			_registeredClients[typeof(TInterface)] = (b,e) => (object)constructor(b,e);
		}
	}
}

#endif
