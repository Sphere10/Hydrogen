//-----------------------------------------------------------------------
// <copyright file="iOSClientBase.cs" company="Sphere 10 Software">
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
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Sphere10.Framework.Services
{
	public abstract class iOSClientBase<T> : ClientBase<T> where T : class {
		private T _channel;

		public iOSClientBase (Binding binding, EndpointAddress address, ILogger logger = null) : base(binding, address) {		
			_channel = null;
		    Logger = logger ?? new NoOpLogger();
		    this.Binding = binding;
		}
        public Binding Binding { get; private set; }
        protected ILogger Logger { get; private set; }
		protected abstract T CreateChannel ();
		
		public T Channel {
			get {
				if (_channel == null)
					_channel = CreateChannel();
				return _channel;
			}
		}

		protected class iOSChannelBase : ChannelBase<T> {
		    private readonly ILogger _logger;
			public iOSChannelBase (iOSClientBase<T> client) : base(client) {
			    _logger = client.Logger;
			    var binding = client.Binding;
			    OperationTimeout = (new[] {binding.OpenTimeout, binding.CloseTimeout, binding.SendTimeout, binding.ReceiveTimeout}).Max();
			}

		    protected void Call(string name, params object[] args) {
                base.Invoke(name, args);
		    }

		    protected U Call<U>(string name, params object[] args) {
		        try {
                    _logger.Debug("iOSChannelBase.Call({0},{1}) -> {2}", name ?? "NULL", args.Select(o => o ?? "NULL").ToDelimittedString(", "), typeof(U).Name);
		            return (U) base.Invoke(name, args);
		        } catch (Exception error) {
                    _logger.LogException(error);
		            throw;
		        }
		    }
		}

	}
}
#endif
