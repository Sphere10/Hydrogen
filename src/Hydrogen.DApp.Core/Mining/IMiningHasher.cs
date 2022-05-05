using System;
using System.Collections.Generic;
using System.Text;
using Hydrogen;
using Hydrogen.DApp.Core.Maths;

namespace Hydrogen.DApp.Core.Mining {
	public interface IMiningHasher {
		string GetDescription();
		byte[] Hash(byte[] input);
	}
}
