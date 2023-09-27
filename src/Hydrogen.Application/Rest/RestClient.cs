// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Hydrogen.Application;

public class RestClient : IRestClient {

	private readonly ILogger _logger;
	private HttpClient _httpClient;

	protected readonly JsonSerializerSettings _defaultSerializerSettings;

	public RestClient(string baseUrl, JsonSerializerSettings defaultSerializerSettings = null, ILogger logger = null) {
		Guard.ArgumentNotNull(baseUrl, nameof(baseUrl));
		BaseUrl = baseUrl;
		_logger = logger ?? new NoOpLogger();
		_defaultSerializerSettings
			= defaultSerializerSettings ??
			  new JsonSerializerSettings {
				  NullValueHandling = NullValueHandling.Ignore,
				  ContractResolver = new DefaultContractResolver {
					  NamingStrategy = new CamelCaseNamingStrategy()
				  }
			  };
	}

	public string BaseUrl { get; }

	public async Task<T> GetAsync<T>(
		string urlPostfix,
		IDictionary<string, string> queryParams = null,
		IDictionary<string, string> headers = null,
		JsonSerializerSettings serializerSettings = null,
		CancellationToken cancellationToken = default) {
		var response = await SendAsync(Tools.Url.Combine(BaseUrl, urlPostfix), HttpMethod.Get, queryParams, headers, cancellationToken: cancellationToken);
		return await response.ParseStreamAsync<T>(serializerSettings);
	}

	public async Task<T> PostAsync<T>(string urlPostfix, object body, IDictionary<string, string> queryParams = null, IDictionary<string, string> headers = null, JsonSerializerSettings responseSerializerSettings = null,
	                                  CancellationToken cancellationToken = default) {
		void AttachContent(HttpRequestMessage httpRequest) {
			httpRequest.Content = new StringContent(JsonConvert.SerializeObject(body, _defaultSerializerSettings), Encoding.UTF8, "application/json");
		}

		var response = await SendAsync(Tools.Url.Combine(BaseUrl, urlPostfix), HttpMethod.Post, queryParams, headers, AttachContent, cancellationToken: cancellationToken);

		return await response.ParseStreamAsync<T>(responseSerializerSettings);
	}

	public async Task<T> PatchAsync<T>(string urlPostfix, object body, IDictionary<string, string> queryParams = null, IDictionary<string, string> headers = null, JsonSerializerSettings responseSerializerSettings = null,
	                                   CancellationToken cancellationToken = default) {
		void AttachContent(HttpRequestMessage httpRequest) {
			var serializedBody = JsonConvert.SerializeObject(body, _defaultSerializerSettings);
			httpRequest.Content = new StringContent(serializedBody, Encoding.UTF8, "application/json");
		}

		var response = await SendAsync(Tools.Url.Combine(BaseUrl, urlPostfix), new HttpMethod("PATCH"), queryParams, headers, AttachContent, cancellationToken: cancellationToken);

		return await response.ParseStreamAsync<T>(responseSerializerSettings);
	}

	public async Task DeleteAsync(string urlPostfix, IDictionary<string, string> queryParams = null, IDictionary<string, string> headers = null, JsonSerializerSettings responseSerializerSettings = null,
	                              CancellationToken cancellationToken = default) {
		await SendAsync(Tools.Url.Combine(BaseUrl, urlPostfix), HttpMethod.Delete, queryParams, headers, null, cancellationToken);
	}

	public async Task<HttpResponseMessage> SendAsync(string url,
	                                                 HttpMethod httpMethod,
	                                                 IDictionary<string, string> queryParams = null,
	                                                 IDictionary<string, string> headers = null,
	                                                 Action<HttpRequestMessage> attachContent = null,
	                                                 CancellationToken cancellationToken = default) {
		EnsureHttpClient();

		url = queryParams == null ? url : Tools.Url.AddQueryString(url, queryParams);

		using var httpRequest = new HttpRequestMessage(httpMethod, url);
		PrepareHttpRequestInternal(httpRequest);

		if (headers != null) {
			foreach (var header in headers) {
				httpRequest.Headers.Add(header.Key, header.Value);
			}
		}

		attachContent?.Invoke(httpRequest);
		var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
		if (!response.IsSuccessStatusCode)
			throw BuildException(response);

		return response;
	}

	protected virtual void PrepareHttpRequestInternal(HttpRequestMessage httpRequest) {
		//httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AuthToken);
		//httpRequest.Headers.Add("Notion-Version", _options.NotionVersion);
	}

	protected virtual Exception BuildException(HttpResponseMessage response) {
		throw new InvalidOperationException($"Error: {response.StatusCode}{Environment.NewLine}{response.Content.ReadAsStringAsync().ResultSafe()}");
	}

	private HttpClient EnsureHttpClient() {
		if (_httpClient == null) {
			var pipeline = new LoggingHandler(_logger, LogLevel.Debug) { InnerHandler = new HttpClientHandler() };
			_httpClient = new HttpClient(pipeline);
			_httpClient.BaseAddress = new Uri(BaseUrl);
		}

		return _httpClient;
	}


	private class LoggingHandler : DelegatingHandler {
		private readonly ILogger _logger;
		private readonly LogLevel _logLevel;

		public LoggingHandler(ILogger logger, LogLevel logLevel = LogLevel.Debug) {
			_logger = logger;
			_logLevel = logLevel;
		}
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			_logger.Log(_logLevel, $"Request: {request}");
			try {
				var response = await base.SendAsync(request, cancellationToken);
				_logger.Log(_logLevel, $"Response: {response}");
				return response;
			} catch (Exception ex) {
				_logger.Exception(ex);
				throw;
			}
		}
	}
}
