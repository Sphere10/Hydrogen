using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications.RPC {
	public class ApiDescriptor {
		protected string									ApiName;		//UniqueID name / Class name. NOTE: does not support anonymous method.
		protected List<ApiMethodDescriptor>					Methods;		//Methods/services description (name, params(type and defult value), return value type). 
	}
}
