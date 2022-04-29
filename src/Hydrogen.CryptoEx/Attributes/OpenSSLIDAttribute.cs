using System;

namespace Sphere10.Framework.CryptoEx {

	public class OpenSSLIDAttribute : Attribute {
		public OpenSSLIDAttribute(int id) {
			ID = id;
		}

		public int ID { get; }
	}

}
