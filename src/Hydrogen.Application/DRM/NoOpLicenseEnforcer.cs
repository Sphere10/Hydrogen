using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydrogen.Application;

namespace Hydrogen.Application;
internal class NoOpLicenseEnforcer : ILicenseEnforcer {
	public int CountAppliedLicense => 0;
	public int CountNagged => 0;

	public void ApplyLicense(bool nagUser = true) {
		// NoOp
	}

	public bool DetermineLicenseCompliance(out bool systemShouldNagUser, out string nagMessage) {
		// NoOp
		systemShouldNagUser = false;
		nagMessage = null;
		return false;
	}

	public ProductRights Rights => ProductRights.None;
}

