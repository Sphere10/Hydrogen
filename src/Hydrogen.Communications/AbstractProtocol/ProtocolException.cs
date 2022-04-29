using System;

namespace Sphere10.Framework.Communications {
    public class ProtocolException : SoftwareException {
		public ProtocolException(string errorMessage, Exception exception = null)
			: base(errorMessage, exception) {
		}
	}
}
