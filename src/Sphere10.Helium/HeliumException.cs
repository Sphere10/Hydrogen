using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sphere10.Framework;

namespace Sphere10.Helium {
	public class HeliumException : SoftwareException {

		public HeliumException() {
		}

		public HeliumException(string error) : base(error) {
		}

		public HeliumException(string error, Exception innerException) : base(error, innerException) {
		}
	}
}
