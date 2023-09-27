// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Communications.RPC;

//Attribute that makes this instance automaticaly registered in the service manager.
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class RpcAPIServiceAttribute : Attribute {
	public RpcAPIServiceAttribute(string apiName) => this.ApiName = apiName;

	public string ApiName { get; private set; }
}


//Attribute that declare this method a callable method via RPC
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class RpcAPIMethodAttribute : Attribute {
	public RpcAPIMethodAttribute(string methodName = "") => this.MethodName = methodName;

	public string MethodName { get; private set; }
}


//Attribute that override the name of a method argument. USefull for simpler/lighter RPC calls
[AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
public sealed class RpcAPIArgumentAttribute : Attribute {
	public RpcAPIArgumentAttribute(string explicitArgumentName) => this.ExplicitArgumentName = explicitArgumentName;

	public string ExplicitArgumentName { get; private set; }
}
