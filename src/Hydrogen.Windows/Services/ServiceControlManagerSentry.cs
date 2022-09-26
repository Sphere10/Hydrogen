using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen.Windows;

public class ServiceControlManagerSentry : ProcessSentry {

	public const string ServiceControlExecutable = "sc.exe";

	public ServiceControlManagerSentry() : base(ServiceControlExecutable) {
	}

	public Task CreateServiceAsync(string name, string executablePath, string startType, CancellationToken cancellationToken = default) {
		Guard.ArgumentNotNull(name, nameof(name));
		Guard.ArgumentNotNull(executablePath, nameof(executablePath));
		Guard.FileExists(executablePath);
		return base.RunWithErrorCodeCheckAsync($"create \"{name}\" binPath=\"{executablePath}\" start={startType}", cancellationToken: cancellationToken);
	}

	public Task RemoveServiceAsync(string name, CancellationToken cancellationToken = default) {
		Guard.ArgumentNotNull(name, nameof(name));
		//Guard.FileExists(executablePath);
		return base.RunWithErrorCodeCheckAsync($"delete \"{name}\"", cancellationToken: cancellationToken);
	}

}