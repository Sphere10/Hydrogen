// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Hydrogen.Communications.RPC;

//Provide host/Local api calls and binding support.
//Can be used as a singleton, a property or an object service
public class ApiService {
	//All suported apis by this service		TODO: make this threadsafe
	protected Dictionary<string, ApiMethodDescriptor> ApiBindings = new Dictionary<string, ApiMethodDescriptor>();

	//Add an entire api using RpcAPI* meta properties in the given object. Note: It does not take the ownership of the instance
	public void AddApi(object instance) {
		//Get api name if any
		var apiNameAttr = instance.GetType().GetCustomAttribute(typeof(RpcAPIServiceAttribute));
		string apiName = apiNameAttr == null ? "" : (apiNameAttr as RpcAPIServiceAttribute).ApiName;

		var methods = instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		foreach (var method in methods) {
			object[] rpcMethods = method.GetCustomAttributes(typeof(RpcAPIMethodAttribute), false);
			if (rpcMethods.Length > 0) {
				//accumulate arguments
				var arguments = new List<Tuple<string, System.Type>>();
				foreach (var parameter in method.GetParameters()) {
					//handle param name override
					object[] nameOverride = parameter.GetCustomAttributes(typeof(RpcAPIArgumentAttribute), false);
					string argName;
					if (nameOverride.Length > 0)
						argName = (nameOverride[0] as RpcAPIArgumentAttribute).ExplicitArgumentName;
					else
						argName = parameter.Name;
					if (string.IsNullOrEmpty(argName))
						continue;
					arguments.Add(new Tuple<string, System.Type>(argName.ToLower(), parameter.ParameterType));
				}

				for (int i = 0; i < rpcMethods.Length; i++) {
					string rpcMethodName = (rpcMethods[i] as RpcAPIMethodAttribute).MethodName == string.Empty ? method.Name : (rpcMethods[i] as RpcAPIMethodAttribute).MethodName;

					//prepend the api name to the method with dot separator
					if (!string.IsNullOrEmpty(apiName))
						rpcMethodName = apiName + "." + rpcMethodName;
					rpcMethodName = rpcMethodName.ToLower();

					if (ApiBindings.TryGetValue(rpcMethodName, out var _))
						throw new Exception($"RPC method '{rpcMethodName}' is allready registered");

					var apiMethodDescr = new ApiMethodDescriptor();
					apiMethodDescr.MethodName = rpcMethodName;
					apiMethodDescr.Arguments = arguments;
					apiMethodDescr.ReturnType = method.ReturnType;

					//create binding with sequential argument types
					var delegatArgType = new List<System.Type>();
					foreach (var t in arguments)
						delegatArgType.Add(t.Item2);

					delegatArgType.Add(method.ReturnType);
					apiMethodDescr.CallPoint = Delegate.CreateDelegate(System.Linq.Expressions.Expression.GetDelegateType(delegatArgType.ToArray()), instance, method);

					ApiBindings.Add(rpcMethodName, apiMethodDescr);
				}
			}
		}
	}

	public bool RemoveApi(object instance) {
		if (instance is ApiServiceProxy)
			instance = (instance as ApiServiceProxy).proxy;

		bool empty = false;
		int count = 0;
		while (!empty) {
			empty = true;
			foreach (var binding in ApiBindings) {
				if (Object.ReferenceEquals(binding.Value.CallPoint.Target, instance)) {
					ApiBindings.Remove(binding.Key);
					empty = false;
					count++;
					continue;
				}
			}
		}
		return count > 0;
	}

	public bool IsApi(object instance) {
		if (instance is ApiServiceProxy)
			instance = (instance as ApiServiceProxy).proxy;
		foreach (var api in ApiBindings)
			if (Object.ReferenceEquals(api.Value.CallPoint.Target, instance))
				return true;

		return false;
	}
	public ApiMethodDescriptor GetMethod(string methodName) {
		ApiMethodDescriptor m = null;
		ApiBindings.TryGetValue(methodName.ToLower(), out m);
		return m;
	}

	//Call actual function from parsed json data
	public JsonResponse Call(string methodName, List<object> arguments) {
		Delegate callPoint = null;
		ApiMethodDescriptor method = null;
		if (ApiBindings.TryGetValue(methodName.ToLower(), out method))
			callPoint = method.CallPoint;
		if (method == null)
			return new JsonResponse { Result = null, Error = new JsonRpcException(-2, $"The method {methodName} does not exist.") };

		//validate and recast arguments
		if (method.Arguments.Count != arguments.Count)
			return new JsonResponse { Result = null, Error = new JsonRpcException(-3, $"Wrong argument count in method {methodName}.") };

		try {
			for (int i = 0; i < method.Arguments.Count; i++) {
				if (arguments[i] is JToken) {
					var argObj = arguments[i] as JToken;

					//special case for byte[] xfer as a hexstring
					if (method.Arguments[i].Item2 == typeof(byte[]))
						arguments[i] = ByteArrayHexConverter.ToByteArray(argObj.ToObject<String>());
					else
						arguments[i] = argObj.ToObject(method.Arguments[i].Item2);

				} else
					return new JsonResponse { Result = null, Error = new JsonRpcException(-8, $"Agument #{i} is incompatible in method {methodName}.") };
			}

		} catch (Exception ex) {
			return new JsonResponse { Result = null, Error = new JsonRpcException(-5, $"Wrong argument type in method {methodName}.") };
		}

		//call delegate
		try {
			var ret = method.CallPoint.DynamicInvoke(arguments.ToArray());
			if (ret is JsonRpcException)
				return new JsonResponse { Result = null, Error = ret as JsonRpcException };
			else if (ret is System.Exception)
				return new JsonResponse { Result = null, Error = new JsonRpcException(-5, $"Call returned exception : {(ret as System.Exception).Message}") };
			else
				return new JsonResponse { Result = ret, Error = null };
		} catch (Exception ex) {
			return new JsonResponse { Result = null, Error = new JsonRpcException(-9, $"Call exception cought : {ex.Message}") };
		}
	}
}
