using System;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Framework {
	public interface ISynchronizedReadOnlyList<T> : IReadOnlyList<T>, ISynchronizedObject {
	}
}
