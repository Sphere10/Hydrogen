using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Sphere10.Framework {

	public enum CHF : ushort {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		ConcatBytes = 0,
		SHA2_256 = 1,
		SHA2_384,
		SHA2_512,
		SHA1_160,
		Blake2b_256,
		Blake2b_160,
		Blake2b_128,
	}

}