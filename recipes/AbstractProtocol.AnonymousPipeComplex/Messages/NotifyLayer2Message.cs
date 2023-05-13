// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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
