using System;

namespace Hydrogen.CryptoEx {

	public class OpenSSLIDAttribute : Attribute {
		public OpenSSLIDAttribute(int id) {
			ID = id;
		}

		public int ID { get; }
	}

}
