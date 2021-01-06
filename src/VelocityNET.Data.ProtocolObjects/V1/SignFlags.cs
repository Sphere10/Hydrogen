using System;

namespace VelocityNET.ProtocolObjects {

	[Flags]
	public enum SignatureGrouping : uint {
		Illegal = 0,
		Default = All,
		OperationGroup1 = 1 << 0,
		OperationGroup2 = 1 << 1,
		OperationGroup3 = 1 << 2,
		OperationGroup4 = 1 << 3,
		OperationGroup5 = 1 << 4,
		OperationGroup6 = 1 << 5,
		OperationGroup7 = 1 << 6,
		OperationGroup8 = 1 << 7,
		OperationGroup9 = 1 << 8,
		OperationGroup10 = 1 << 9,
		OperationGroup11 = 1 << 10,
		OperationGroup12 = 1 << 11,
		OperationGroup13 = 1 << 12,
		OperationGroup14 = 1 << 13,
		OperationGroup15 = 1 << 14,
		OperationGroup16 = 1 << 15,
		OperationGroup17 = 1 << 16,
		OperationGroup18 = 1 << 17,
		OperationGroup19 = 1 << 18,
		OperationGroup20 = 1 << 19,
		OperationGroup21 = 1 << 20,
		OperationGroup22 = 1 << 21,
		OperationGroup23 = 1 << 22,
		OperationGroup24 = 1 << 23,
		OperationGroup25 = 1 << 24,
		OperationGroup26 = 1 << 25,
		OperationGroup27 = 1 << 26,
		OperationGroup28 = 1 << 27,
		OperationGroup29 = 1 << 28,
		OperationGroup30 = 1 << 29,
		OperationGroup31 = 1 << 30,
		OperationGroup32 = 1U << 31,
		All = OperationGroup1 | OperationGroup2 | OperationGroup3 | OperationGroup4 | OperationGroup5 | OperationGroup6 | OperationGroup7 | OperationGroup8 | OperationGroup9 | OperationGroup10 | OperationGroup11 | OperationGroup12 | OperationGroup13 | OperationGroup14 | OperationGroup15 | OperationGroup16 | OperationGroup17 | OperationGroup18 | OperationGroup19 | OperationGroup20 | OperationGroup21 | OperationGroup22 | OperationGroup23 | OperationGroup24 | OperationGroup25 | OperationGroup26 | OperationGroup27 | OperationGroup28 | OperationGroup29 | OperationGroup30 | OperationGroup31
	}

}