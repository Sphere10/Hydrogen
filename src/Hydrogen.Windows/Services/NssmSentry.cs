using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen.Windows;

/// <summary>
/// A process sentry for the Non-sucking Service Manager (NSSM). It is assumed NSSM is in the environment path.
/// </summary>
public class NssmSentry : ProcessSentry {

	public const string ExecutableFileName = "nssm.exe";

	public NssmSentry() : base(ExecutableFileName) {
	}

	public new static Task<bool> CanRunAsync(CancellationToken cancellationToken = default)  
		=> ProcessSentry.CanRunAsync(ExecutableFileName, cancellationToken);

	public Task InstallAsync(string serviceName, string appFileName, string appArgs, CancellationToken cancellationToken = default) {
		Guard.ArgumentNotNull(serviceName, nameof(serviceName));
		Guard.ArgumentNotNull(appFileName, nameof(appFileName));
		Guard.ArgumentNotNull(appArgs, nameof(appArgs));

		return base.RunWithErrorCodeCheckAsync($"install \"{serviceName}\" \"{appFileName}\" {appArgs}", cancellationToken: cancellationToken);
	}

	public Task RemoveAsync(string name, CancellationToken cancellationToken = default) {
		Guard.ArgumentNotNull(name, nameof(name));
		return base.RunWithErrorCodeCheckAsync($"remove \"{name}\"", cancellationToken: cancellationToken);
	}

	public Task StartAsync(string name, CancellationToken cancellationToken = default) {
		Guard.ArgumentNotNull(name, nameof(name));
		return base.RunWithErrorCodeCheckAsync($"start \"{name}\"", cancellationToken: cancellationToken);
	}


	public Task StopAsync(string name, CancellationToken cancellationToken = default) {
		Guard.ArgumentNotNull(name, nameof(name));
		return base.RunWithErrorCodeCheckAsync($"stop \"{name}\"", cancellationToken: cancellationToken);
	}
}