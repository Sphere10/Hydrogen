using Sphere10.Framework;
using System;

namespace AbstractProtocol.AnonymousPipeComplex {

	[Serializable]
	public class NotifyNewBlock {
		public Guid GlobalID { get; set; }
		public string Value { get; set; }
		internal static NotifyNewBlock GenRandom() => new() { GlobalID = Guid.NewGuid(), Value = Tools.Maths.RNG.NextString(20, 20) };

	}

}
