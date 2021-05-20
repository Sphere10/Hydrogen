using System;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sphere10.Framework.Communications.RPC {
	//Provide support classes that do not derive ApiService so they can be represented as an ApiService
	public class ApiServiceProxy: ApiService {
		public object proxy = null;
	}
}
