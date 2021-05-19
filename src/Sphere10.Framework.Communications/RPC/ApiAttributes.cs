using System;
using System.Reflection;

namespace Sphere10.Framework.Communications.RPC {

	//Attribute that makes this instance automaticaly registered in the service manager.
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class RpcAPIServiceAttribute : Attribute {
		public readonly string apiName;
		public RpcAPIServiceAttribute(string apiName) { this.apiName = apiName; }
	}

	//Attribute that declare this method a callable method via RPC
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	public sealed class RpcAPIMethodAttribute : Attribute {
		public readonly string methodName;
		public RpcAPIMethodAttribute(string methodName = "") { this.methodName = methodName; }
	}

	//Attribute that override the name of a method argument. USefull for simpler/lighter RPC calls
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
	public sealed class RpcAPIArgumentAttribute : Attribute {
		public readonly string explicitArgumentName;
		public RpcAPIArgumentAttribute(string explicitArgumentName) { this.explicitArgumentName = explicitArgumentName; }
	}
}
