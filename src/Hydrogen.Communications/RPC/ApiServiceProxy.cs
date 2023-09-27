// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Communications.RPC;

//Provide support classes that do not derive ApiService so they can be represented as an ApiService [As-A relation]
public class ApiServiceProxy : ApiService {
	public object proxy = null;
}
