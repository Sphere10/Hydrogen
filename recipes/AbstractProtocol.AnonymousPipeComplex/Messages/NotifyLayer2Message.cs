using System;

namespace AbstractProtocol.AnonymousPipeComplex {

	[Serializable]
	public class NotifyLayer2Message {
		public int ID { get; set; }
		public NotifyNewTransaction SubMessage1 { get; set; }
		public NotifyNewBlock SubMessage2 { get; set; }
		internal static NotifyLayer2Message GenRandom() => new() { ID = Tools.Maths.RNG.Next(0, 3), SubMessage1 = NotifyNewTransaction.GenRandom(), SubMessage2 = NotifyNewBlock.GenRandom() };
	}

}
