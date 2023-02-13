using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydrogen;

namespace Hydrogen.Web.AspNetCore;
public class HydrogenLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider {

	public HydrogenLoggerProvider(Hydrogen.ILogger hydrogenLogger) {
		HydrogenLogger = hydrogenLogger;
	}

	protected ILogger HydrogenLogger { get; }

	public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
		=> new MicrosoftExtensionsLoggerAdapter(HydrogenLogger);

	public void Dispose() {
	}

}