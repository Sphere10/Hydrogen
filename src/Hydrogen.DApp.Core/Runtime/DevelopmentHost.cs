using System;
using System.Threading.Tasks;
using Sphere10.Framework;

namespace Sphere10.Hydrogen.Core.Runtime {
	public class DevelopmentHost : Host {

		public DevelopmentHost(ILogger logger) 
			: this(logger, new DevelopmentApplicationPaths()) {
		}

		public DevelopmentHost(ILogger logger, IApplicationPaths paths)
			: base(logger, paths) {
		}

		public override Task DeployHAP(string newHapPath) {
			throw new NotSupportedException("Deployment is not supported in development mode");
		}

	}
}