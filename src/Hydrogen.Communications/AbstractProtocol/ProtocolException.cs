using System;

namespace Hydrogen.Communications {
    public class ProtocolException : SoftwareException {
		public ProtocolException(string errorMessage, Exception exception = null)
			: base(errorMessage, exception) {
		}
	}
}
