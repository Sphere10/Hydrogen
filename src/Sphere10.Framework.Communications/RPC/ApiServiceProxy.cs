﻿using System;

namespace Sphere10.Framework.Communications.RPC {
	//Provide support classes that do not derive ApiService so they can be represented as an ApiService [As-A relation]
	public class ApiServiceProxy: ApiService {
		public object proxy = null;
	}
}
