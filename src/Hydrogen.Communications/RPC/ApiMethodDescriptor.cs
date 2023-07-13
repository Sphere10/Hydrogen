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

public class ApiMethodDescriptor {
	//Service or Method name
	public string MethodName;

	//argument name and type, ordered
	public List<Tuple<string, System.Type>> Arguments = new List<Tuple<string, System.Type>>();

	//return type. 
	public System.Type ReturnType;

	//the actual instance's method callpoint
	public Delegate CallPoint;
}
