﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework {
	public interface ITokenResolver {
		string TryResolve(string token);
	}
}