using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace Hydrogen;


public interface IRestClient {
	Task<T> GetAsync<T>(
		string uri,
		IDictionary<string, string> queryParams = null,
		IDictionary<string, string> headers = null,
		JsonSerializerSettings serializerSettings = null,
		CancellationToken cancellationToken = default);

	Task<T> PostAsync<T>(
		string uri,
		object body,
		IDictionary<string, string> queryParams = null,
		IDictionary<string, string> headers = null,
		JsonSerializerSettings responseSerializerSettings = null,
		CancellationToken cancellationToken = default);

	Task<T> PatchAsync<T>(
		string uri,
		object body,
		IDictionary<string, string> queryParams = null,
		IDictionary<string, string> headers = null,
		JsonSerializerSettings responseSerializerSettings = null,
		CancellationToken cancellationToken = default);

	Task DeleteAsync(
		string uri,
		IDictionary<string, string> queryParams = null,
		IDictionary<string, string> headers = null,
		JsonSerializerSettings responseSerializerSettings = null,
		CancellationToken cancellationToken = default);
}