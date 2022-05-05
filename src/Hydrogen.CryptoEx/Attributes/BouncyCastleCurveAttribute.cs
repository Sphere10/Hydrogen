using System;

namespace Hydrogen.CryptoEx {

	public class BouncyCastleCurveAttribute : Attribute {
		public BouncyCastleCurveAttribute(string name) {
			Name = name;
		}

		public string Name { get; }
	}

}
