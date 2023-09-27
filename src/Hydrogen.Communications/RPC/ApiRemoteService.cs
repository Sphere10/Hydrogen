// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Communications.RPC;

//Provide Remote and Local api calls support
public abstract class ApiRemoteService : ApiService {
	protected IEndPoint EndPoint; //Actual endpoint (TCP, NamedPIPE, POST, REST...)


	public delegate Exception ErrorHandler(); //for extra errors handling    


	public ApiRemoteService(IEndPoint endpoint) => EndPoint = endpoint;

	//Remote Api support
	public abstract void RemoteCall(string methodName, params object[] arguments);

	//Call RPC function with a return value
	public abstract TReturnType RemoteCall<TReturnType>(string methodName, params object[] arguments);

	//Call RPC function in batch
	public abstract object[] RemoteCall(ApiBatchCallDescriptor batchOfCalls);
}
