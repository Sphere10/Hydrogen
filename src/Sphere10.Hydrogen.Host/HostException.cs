using System;
using Sphere10.Framework;

namespace Sphere10.Hydrogen.Host {
	public class HostException : SoftwareException {
		public HostException(string errorMessage, Exception exception = null)
			: base(errorMessage, exception) {
		}
	}
}
