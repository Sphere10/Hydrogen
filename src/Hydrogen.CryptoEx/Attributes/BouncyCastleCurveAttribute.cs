using System;

namespace Sphere10.Framework.CryptoEx {

	public class BouncyCastleCurveAttribute : Attribute {
		public BouncyCastleCurveAttribute(string name) {
			Name = name;
		}

		public string Name { get; }
	}

}
