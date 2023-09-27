// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.Communications.RPC;

//batch object for batch of remote function calls
public class ApiBatchCallDescriptor {
	public List<Tuple<System.Type, string, object[]>> FunctionCalls = new List<Tuple<System.Type, string, object[]>>();

	//helper to avoid mega initializer with many new inside of them
	public void Call<T>(string methodName, params object[] arguments) => FunctionCalls.Add(new Tuple<System.Type, string, object[]>(typeof(T), methodName, arguments));

	public void Call(string methodName, params object[] arguments) => FunctionCalls.Add(new Tuple<System.Type, string, object[]>(typeof(Void), methodName, arguments));
}
