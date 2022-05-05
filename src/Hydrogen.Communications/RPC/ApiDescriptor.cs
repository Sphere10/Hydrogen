using System;
using System.Collections.Generic;

namespace Hydrogen.Communications.RPC {
	public class ApiDescriptor {
		//UniqueID name / Class name. NOTE: does not support anonymous method.
		protected string ApiName;
		//Methods/services description (name, params(type and defult value), return value type). 
		protected List<ApiMethodDescriptor> Methods;
	}
}
