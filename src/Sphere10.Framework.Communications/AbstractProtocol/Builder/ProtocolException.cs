
using System;

namespace Sphere10.Framework.Communications {
    public class ProtocolBuilderException : SoftwareException {

		public ProtocolBuilderException(string error) : base(error) {
		}

		public ProtocolBuilderException(string error, Exception innerException) : base(error, innerException) {
		}

	}

}
