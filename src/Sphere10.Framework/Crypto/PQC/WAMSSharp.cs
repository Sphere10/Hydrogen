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