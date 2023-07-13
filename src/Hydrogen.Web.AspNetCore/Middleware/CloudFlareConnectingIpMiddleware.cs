using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace Hydrogen.Web.AspNetCore;

public class CloudflareConnectingIPMiddleware {
	private readonly RequestDelegate _next;

	public CloudflareConnectingIPMiddleware(RequestDelegate next) {
		_next = next;
	}

	public async Task Invoke(HttpContext context) {
		if (context.Request.Headers.TryGetValue("cf-connecting-ip", out var values)) {
			var ipAddress = values.FirstOrDefault();
			if (!string.IsNullOrEmpty(ipAddress)) {
				context.Connection.RemoteIpAddress = IPAddress.Parse(ipAddress);
			}
		}

		await _next(context);
	}
}
