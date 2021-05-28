using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications.RPC {
	//Service Manager Singleton
	public class ApiServiceManager {
		private static Dictionary<string, ApiService> Services = new Dictionary<string, ApiService>();     //TODO: threadsafe this
		static protected object _mutex = new object();

		//Register an object with RpcAPIService attribute.
		//If the object is not an APIService, the manager will create one as a placeholder.
		public static void RegisterService(object service) {
			lock (_mutex) {
				var apiNameAttr = service.GetType().GetCustomAttribute(typeof(RpcAPIServiceAttribute));
				string apiName = apiNameAttr == null ? "" : (apiNameAttr as RpcAPIServiceAttribute).apiName;
				apiName = apiName.ToLower();

				ApiService api = null;
				Services.TryGetValue(apiName, out api);
				//Only allow anonymous api to "re-register" in orer to add some new RPC methods
				if (api != null && apiName.Length != 0)
					throw new Exception($"RPC service '{apiName}' is allready registered");

				//Handle anon api
				if (api != null)
					api.AddApi(service);
				else {
					//Add new service
					ApiService apiService = service as ApiService;
					if (apiService == null)
						apiService = new ApiServiceProxy { proxy = service };

					apiService.AddApi(service);
					Services.Add(apiName, apiService);
				}
			}
		}

		public static ApiService GetService(string serviceName) {
			lock (_mutex) {
				ApiService service;
				if (Services.TryGetValue(serviceName.ToLower(), out service))
					return service;
				else
					return null;
			}
		}

		//handle 'methodname' or 'service.methodname'
		public static ApiService GetServiceFromMethod(string methodName) {
			lock (_mutex) {
				string[] sm = methodName.Split('.', 2, System.StringSplitOptions.TrimEntries);
				if (sm.Length == 2)
					return GetService(sm[0]);

				//return anonymous api if no apiname provided
				return GetService("");
			}
		}
		//Unregister service by it's name. One service instance for one name.
		public static void UnregisterService(string serviceName) {
			lock (_mutex) {
				foreach (var s in Services) {
					if (s.Key == serviceName.ToLower()) {
						s.Value.RemoveApi(s.Value);
						Services.Remove(s.Key);
						return;
					}
				}
			}
		}
		//Unregister service instance that is potentially inside another service (ex. anonymous RPC methods)
		public static void UnregisterService(object service) {
			lock (_mutex) {
				var toRemove = new List<string>();
				foreach (var s in Services) {
					if (s.Value.RemoveApi(service))
						toRemove.Add(s.Key);
				}
				foreach (var key in toRemove)
					Services.Remove(key);
			}
		}
	}
}

