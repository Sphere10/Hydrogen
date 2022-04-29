using Sphere10.Framework;
using System;

namespace AbstractProtocol.AnonymousPipeComplex {

        [Serializable]
		public class NotifyNewTransaction {
			public string Name { get; set; }
			public int Age { get; set; }
			internal static NotifyNewTransaction GenRandom() => new() { Name = Guid.NewGuid().ToStrictAlphaString(), Age = Tools.Maths.RNG.Next(5, 75) };
		}

}
