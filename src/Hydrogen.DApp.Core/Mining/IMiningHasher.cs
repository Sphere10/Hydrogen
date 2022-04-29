using System;
using System.Collections.Generic;
using System.Text;
using Sphere10.Framework;
using Sphere10.Hydrogen.Core.Maths;

namespace Sphere10.Hydrogen.Core.Mining {
	public interface IMiningHasher {
		string GetDescription();
		byte[] Hash(byte[] input);
	}
}
