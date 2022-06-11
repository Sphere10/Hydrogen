using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen {
	public interface ITokenResolver {
		bool TryResolve(string token, out string value);
	}
}
