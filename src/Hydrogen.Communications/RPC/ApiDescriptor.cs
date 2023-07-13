// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen.Communications.RPC;

public class ApiDescriptor {
	//UniqueID name / Class name. NOTE: does not support anonymous method.
	protected string ApiName;

	//Methods/services description (name, params(type and defult value), return value type). 
	protected List<ApiMethodDescriptor> Methods;
}
