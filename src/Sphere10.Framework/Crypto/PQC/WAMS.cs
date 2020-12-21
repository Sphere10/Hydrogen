using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework {

	public class WAMS : AMS {

		public WAMS() 
			: this(Configuration.DefaultHeight) {
		}

		public WAMS(int h) 
			: this(h, WOTS.Configuration.Default.W, CHF.SHA2_256) {
		}

		public WAMS(int h, int w)
			: this(h, w, CHF.SHA2_256) {
		}

		public WAMS(int h, int w, CHF chf) 
			: base(new WOTS(new WOTS.Configuration(w, chf, true)), h) {
		}

	}
}