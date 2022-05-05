namespace Hydrogen {

	public class WAMSSharp : AMS {

		public WAMSSharp() 
			: this(Configuration.DefaultHeight) {
		}

		public WAMSSharp(int h) 
			: this(h, WOTSSharp.Configuration.Default.W) {
		}

		public WAMSSharp(int h, int w)
			: this(h, w, WOTSSharp.Configuration.Default.HashFunction) {
		}

		public WAMSSharp(int h, int w, CHF chf) 
			: base(new WOTSSharp(new WOTSSharp.Configuration(w, chf, true)), h) {
		}

	}
}