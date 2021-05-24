﻿using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications.RPC {
	//Provide Remote and Local api calls support
	public abstract class ApiRemoteService: ApiService {
		protected IEndPoint EndPoint;				//Actual endpoint (TCP, NamedPIPE, POST, REST...)
		public delegate	Exception ErrorHandler();	//for extra errors handling    

		public ApiRemoteService(IEndPoint endpoint) => EndPoint = endpoint;

		//Remote Api support
		public abstract void RemoteCall(string methodName, params object[] arguments);
		//Call RPC function with a return value
		public abstract TReturnType RemoteCall<TReturnType>(string methodName, params object[] arguments);
	}
}