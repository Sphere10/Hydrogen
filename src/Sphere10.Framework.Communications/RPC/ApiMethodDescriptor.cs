using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications.RPC {
	public class ApiMethodDescriptor {
		//Service or Method name
		public string							MethodName;
		//argument name and type, ordered
		public List<Tuple<string, System.Type>>	Arguments = new List<Tuple<string, System.Type>>();
		//return type. 
		public System.Type						ReturnType;
		//the actual instance's method callpoint
		public Delegate							CallPoint;
	}
}