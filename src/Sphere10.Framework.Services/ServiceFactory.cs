//-----------------------------------------------------------------------
// <copyright file="ServiceFactory.cs" company="Sphere 10 Software">
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
using System.ServiceModel;
using System.ServiceModel.Channels;


namespace Sphere10.Framework.Services {

	public class ServiceFactory<T> : ChannelFactory<T> where T : class {


		public ServiceFactory (Binding binding, EndpointAddress remoteAddresss) : base(binding, remoteAddresss) {
			Binding = binding;
		}
		
		public override T CreateChannel (EndpointAddress address, Uri via)
		{

			T channel = default(T);
#if !__IOS__
			channel = base.CreateChannel(address, via);
#else
			if (!iOSChannelRegister.ContainsClientFor<T>())
				throw new SoftwareException("No registered channel for {0}", typeof(T).Name);
			channel = iOSChannelRegister.CreateClient<T>(this.Binding, address);
#endif
			return channel;
			
		}

		public Binding Binding {
			get;
			private set;
		}
	}



}




/*
#if __IOS__

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using BW.Cranewatch.Services.Contracts;

namespace BW.Cranewatch.Services.Client
{


	public class ViewModelProviderService : ClientBase<IViewModelProvider>
	{

		private class ViewModelProviderChannel : ChannelBase<IViewModelProvider>, IViewModelProvider
		{
	       
			public ViewModelProviderChannel (ClientBase<IViewModelProvider> client) :
		            base(client)
			{
			}


			public Result<MainScreenDTO> GetMainScreenViewModel (Guid accessToken)
			{
				object[] _args = new object[1];
				_args [0] = accessToken;
				return (Result<MainScreenDTO>)base.Invoke ("GetMainScreenViewModel", _args);
		        
			}

			public MainScreenDTO GetMainScreenViewModel2 (Guid accessToken)
			{
				object[] _args = new object[1];
				_args [0] = accessToken;
				return (MainScreenDTO)base.Invoke ("GetAccountBalance", _args);
			}

			public string Test ()
			{
				object[] _args = new object[0];
				return (string)base.Invoke ("GetAccountBalance", _args);
			}

		}


		public ViewModelProviderService (Binding binding, EndpointAddress address) : base(binding, address)
		{
				
		}

		protected override IViewModelProvider CreateChannel ()
		{
			return new ViewModelProviderChannel (this);
		}

		public IViewModelProvider Channel {
			get {
				return base.Channel;
			}
		}



	}
}



*/
