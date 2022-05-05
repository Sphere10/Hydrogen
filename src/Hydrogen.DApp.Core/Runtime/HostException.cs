using System;
using Hydrogen;

namespace Hydrogen.DApp.Core.Runtime {
	public class HostException : SoftwareException {
		public HostException(string errorMessage, Exception exception = null)
			: base(errorMessage, exception) {
		}
	}
}
