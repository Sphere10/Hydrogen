//-----------------------------------------------------------------------
// <copyright file="ServiceClient.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;


namespace Sphere10.Framework.Services {

    public class ServiceClient<T> : IDisposable where T : class {
		
        public ServiceClient(string uri) : this(uri, BindingFactory.CreateStandardBinding()) {
        }

        public ServiceClient(string uri, Binding binding) {
            if (!typeof(T).IsInterface) {
                throw new ApplicationException("BaseClient generic parameter must be an interface type");
            }

            // Any channel setup code goes here
            var address = new EndpointAddress(uri);
#if !__IOS__
            var factory = new ChannelFactory<T>(binding, address);
            Service = factory.CreateChannel();
            ((IClientChannel) Service).OperationTimeout = (new[] {binding.OpenTimeout, binding.CloseTimeout, binding.SendTimeout, binding.ReceiveTimeout}).Max();
#else
				if (!iOSChannelRegister.ContainsClientFor<T>())
					throw new SoftwareException("No registered channel for {0}", typeof(T).Name);
				Service = iOSChannelRegister.CreateClient<T>(binding, address);
#endif
        }

        public T Service { get; private set; }

        public void Dispose() {
            if (Service is IDisposable) {
                ((IDisposable)Service).Dispose();
            }
        }
    }
}
