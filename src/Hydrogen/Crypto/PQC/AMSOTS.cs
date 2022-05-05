using System.ComponentModel;

namespace Hydrogen {

	public enum AMSOTS : byte{
		[Description("N/A")]
		NotApplicable = 0,
		
		[Description("W-OTS")]
		WOTS = 1,

		[Description("W-OTS+")]
		WOTS_Plus = 2,

		[Description("W-OTS#")]
		WOTS_Sharp = 3,
	}

}
