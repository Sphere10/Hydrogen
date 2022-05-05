﻿using System;
using System.Collections.Generic;

namespace Hydrogen.Communications.RPC {

	//batch object for batch of remote function calls
	public class ApiBatchCallDescriptor {
		public List<Tuple<System.Type, string, object[]>> FunctionCalls = new List<Tuple<System.Type, string, object[]>>();

		//helper to avoid mega initializer with many new inside of them
		public void Call<T>(string methodName, params object[] arguments) => FunctionCalls.Add(new Tuple<System.Type, string, object[]>(typeof(T), methodName, arguments)); 
		public void Call(string methodName, params object[] arguments) => FunctionCalls.Add(new Tuple<System.Type, string, object[]>(typeof(Void), methodName, arguments)); 
	}		
}
