using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen {
	public interface ITokenResolver {
		string TryResolve(string token);
	}
}
